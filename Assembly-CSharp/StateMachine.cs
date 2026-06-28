using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

public abstract class StateMachine
{
	public StateMachine()
	{
		this.name = base.GetType().FullName;
		this.debugSettings = StateMachineDebuggerSettings.Get().CreateEntry(base.GetType());
	}

	public abstract string[] GetStateNames();

	public abstract StateMachine.BaseState GetState(string name);

	public abstract void BindStates();

	public int version { get; protected set; }

	public bool serializable { get; protected set; }

	public virtual void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = null;
	}

	public void InitializeStateMachine()
	{
		StateMachine.BaseState baseState = null;
		this.CreateStates(this);
		this.BindStates();
		this.InitializeStates(out baseState);
		DebugUtil.Assert(baseState != null, "Assert!");
		this.defaultState = baseState;
	}

	private void CreateStates(object state_machine)
	{
		Type type = state_machine.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo fieldInfo in fields)
		{
			bool flag = false;
			foreach (object obj in fieldInfo.GetCustomAttributes(false))
			{
				if (obj.GetType() == typeof(StateMachine.DoNotAutoCreate))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (fieldInfo.FieldType.IsSubclassOf(typeof(StateMachine.BaseState)))
				{
					StateMachine.BaseState baseState = (StateMachine.BaseState)Activator.CreateInstance(fieldInfo.FieldType);
					this.CreateStates(baseState);
					fieldInfo.SetValue(state_machine, baseState);
				}
				else if (fieldInfo.FieldType.IsSubclassOf(typeof(StateMachine.Parameter)))
				{
					StateMachine.Parameter parameter = (StateMachine.Parameter)fieldInfo.GetValue(state_machine);
					if (parameter == null)
					{
						parameter = (StateMachine.Parameter)Activator.CreateInstance(fieldInfo.FieldType);
						fieldInfo.SetValue(state_machine, parameter);
					}
					parameter.name = fieldInfo.Name;
					parameter.idx = this.parameters.Length;
					this.parameters = this.parameters.Append(parameter);
				}
				else if (fieldInfo.FieldType.IsSubclassOf(typeof(StateMachine)))
				{
					fieldInfo.SetValue(state_machine, this);
				}
			}
		}
	}

	public StateMachine.BaseState GetDefaultState()
	{
		return this.defaultState;
	}

	public int GetMaxDepth()
	{
		return this.maxDepth;
	}

	public override string ToString()
	{
		return this.name;
	}

	protected string name;

	protected int maxDepth;

	protected StateMachine.BaseState defaultState;

	protected StateMachine.Parameter[] parameters = new StateMachine.Parameter[0];

	public int dataTableSize;

	public StateMachineDebuggerSettings.Entry debugSettings;

	public bool saveHistory;

	public sealed class DoNotAutoCreate : Attribute
	{
	}

	public enum Status
	{
		Initialized,
		Running,
		Failed,
		Success
	}

	public class Category : Resource
	{
		public Category(string id)
			: base(id, null, null)
		{
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public abstract class Instance
	{
		public Instance(StateMachine state_machine, IStateMachineTarget master)
		{
			this.stateMachine = state_machine;
			this.CreateParameterContexts();
			this.log = new LoggerFS(this.stateMachine.name);
		}

		public abstract void Update();

		public abstract StateMachine.BaseState GetCurrentState();

		public abstract void GoTo(StateMachine.BaseState state);

		public abstract float timeinstate { get; }

		public abstract float deltatime { get; }

		public abstract float dt { get; }

		public abstract IStateMachineTarget GetMaster();

		public abstract void StopSM(string reason);

		public abstract SchedulerHandle Schedule(float time, Action<object> callback, object callback_data = null);

		public bool IsRunning()
		{
			return this.GetCurrentState() != null;
		}

		public void GoTo(string state_name)
		{
			StateMachine.BaseState state = this.stateMachine.GetState(state_name);
			this.GoTo(state);
		}

		public int GetStackSize()
		{
			return this.stackSize;
		}

		public StateMachine GetStateMachine()
		{
			return this.stateMachine;
		}

		public void Log(string message)
		{
			if (this.IsConsoleLoggingEnabled())
			{
				Debug.Log(this.GetMaster().name + "." + message);
			}
		}

		public bool IsConsoleLoggingEnabled()
		{
			return this.enableConsoleLogging || this.stateMachine.debugSettings.enableConsoleLogging;
		}

		public bool IsBreakOnGoToEnabled()
		{
			return this.breakOnGoTo || this.stateMachine.debugSettings.breakOnGoTo;
		}

		public LoggerFS GetLog()
		{
			return this.log;
		}

		public StateMachine.Parameter.Context[] GetParameterContexts()
		{
			return this.parameterContexts;
		}

		public StateMachine.Parameter.Context GetParameterContext(StateMachine.Parameter parameter)
		{
			return this.parameterContexts[parameter.idx];
		}

		public StateMachine.Status GetStatus()
		{
			return this.status;
		}

		public void SetStatus(StateMachine.Status status)
		{
			this.status = status;
		}

		public void Error()
		{
			if (!StateMachine.Instance.error)
			{
				this.isCrashed = true;
				StateMachine.Instance.error = true;
				RestartWarning.ShouldWarn = true;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (this.GetCurrentState() != null)
			{
				text = this.GetCurrentState().name;
			}
			else if (this.GetStatus() != StateMachine.Status.Initialized)
			{
				text = this.GetStatus().ToString();
			}
			return this.stateMachine.ToString() + "(" + text + ")";
		}

		public virtual void StartSM()
		{
			if (!this.IsRunning())
			{
				StateMachine.BaseState defaultState = this.stateMachine.GetDefaultState();
				DebugUtil.Assert(defaultState != null, "Assert!");
				if (this.GetStateMachine().serializable)
				{
					if (!this.GetComponent<StateMachineController>().Restore(this))
					{
						this.GoTo(defaultState);
					}
				}
				else
				{
					this.GoTo(defaultState);
				}
			}
		}

		public bool HasTag(Tag tag)
		{
			StateMachine.BaseState currentState = this.GetCurrentState();
			if (currentState == null)
			{
				return false;
			}
			for (int i = 0; i < currentState.branch.Length; i++)
			{
				if (currentState.branch[i].HasTag(tag))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInsideState(StateMachine.BaseState state)
		{
			StateMachine.BaseState currentState = this.GetCurrentState();
			if (currentState == null)
			{
				return false;
			}
			for (int i = 0; i < currentState.branch.Length; i++)
			{
				if (state == currentState.branch[i])
				{
					return true;
				}
			}
			return false;
		}

		public void ScheduleGoTo(float time, StateMachine.BaseState state)
		{
			if (this.scheduleGoToCallback == null)
			{
				this.scheduleGoToCallback = delegate(object d)
				{
					this.GoTo((StateMachine.BaseState)d);
				};
			}
			this.Schedule(time, this.scheduleGoToCallback, state);
		}

		public void Subscribe(int hash, EventSystem.EventHandler handler)
		{
			this.GetMaster().Subscribe(hash, handler);
		}

		public void Unsubscribe(int hash, EventSystem.EventHandler handler)
		{
			this.GetMaster().Unsubscribe(hash, handler);
		}

		public void Trigger(int hash, object data = null)
		{
			this.GetMaster().GetComponent<KPrefabID>().Trigger(hash, data);
		}

		public ComponentType Get<ComponentType>()
		{
			return this.GetComponent<ComponentType>();
		}

		public ComponentType GetComponent<ComponentType>()
		{
			return this.GetMaster().GetComponent<ComponentType>();
		}

		private void CreateParameterContexts()
		{
			this.parameterContexts = new StateMachine.Parameter.Context[this.stateMachine.parameters.Length];
			for (int i = 0; i < this.stateMachine.parameters.Length; i++)
			{
				this.parameterContexts[i] = this.stateMachine.parameters[i].CreateContext();
			}
		}

		public GameObject gameObject
		{
			get
			{
				return this.GetMaster().gameObject;
			}
		}

		public Transform transform
		{
			get
			{
				return this.gameObject.transform;
			}
		}

		public const float UPDATE_TIME = 0.2f;

		protected LoggerFS log;

		protected StateMachine.Status status;

		protected StateMachine stateMachine;

		protected Stack<StateEvent.Context> subscribedEvents = new Stack<StateEvent.Context>();

		protected int stackSize;

		protected StateMachine.Parameter.Context[] parameterContexts;

		public object[] dataTable;

		private Action<object> scheduleGoToCallback;

		protected int updatingStateCount;

		public Action<string, StateMachine.Status> OnStop;

		public bool breakOnGoTo;

		public bool enableConsoleLogging;

		public bool isCrashed;

		public static bool error;
	}

	public class BaseState
	{
		public BaseState()
		{
			this.branch = new StateMachine.BaseState[1];
			this.branch[0] = this;
		}

		public int GetStateCount()
		{
			return this.branch.Length;
		}

		public StateMachine.BaseState GetState(int idx)
		{
			return this.branch[idx];
		}

		public bool HasTag(Tag tag)
		{
			if (this.tags != null)
			{
				for (int i = 0; i < this.tags.Length; i++)
				{
					if (this.tags[i] == tag)
					{
						return true;
					}
				}
			}
			return false;
		}

		public string name;

		public string longName;

		public string debugPushName;

		public string debugPopName;

		public string debugExecuteName;

		public StateMachine.BaseState defaultState;

		public StateEvent[] events;

		public StateMachine.BaseTransition[] transitions;

		public StateMachine.ParameterTransition[] parameterTransitions;

		public StateMachine.Action[] enterActions;

		public StateMachine.Action[] exitActions;

		public StateMachine.Action[] updateActions;

		public StateMachine.BaseState[] branch;

		public StateMachine.BaseState parent;

		public Tag[] tags;
	}

	public class BaseTransition
	{
		public BaseTransition(string name, StateMachine.BaseState target_state)
		{
			this.name = name;
			this.targetState = target_state;
		}

		public string name;

		public StateMachine.BaseState targetState;
	}

	public class Action
	{
		public Action(string name, string log_name, object callback)
		{
			this.name = name;
			this.logName = log_name;
			this.prefixedLogName = "." + log_name;
			this.callback = callback;
		}

		public string name;

		public string logName;

		public string prefixedLogName;

		public object callback;
	}

	public class ParameterTransition
	{
	}

	public abstract class Parameter
	{
		public abstract StateMachine.Parameter.Context CreateContext();

		public string name;

		public int idx;

		public abstract class Context
		{
			public Context(StateMachine.Parameter parameter)
			{
				this.parameter = parameter;
			}

			public abstract void Serialize(BinaryWriter writer);

			public abstract void Deserialize(IReader reader);

			public abstract void ShowEditor(StateMachine.Instance base_smi);

			public StateMachine.Parameter parameter;
		}
	}
}
