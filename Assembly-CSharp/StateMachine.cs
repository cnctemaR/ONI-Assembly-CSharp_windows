using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

public abstract class StateMachine
{
	public StateMachine()
	{
		this.name = base.GetType().FullName;
	}

	public virtual void FreeResources()
	{
		this.name = null;
		if (this.defaultState != null)
		{
			this.defaultState.FreeResources();
		}
		this.defaultState = null;
		this.parameters = null;
	}

	public abstract string[] GetStateNames();

	public abstract StateMachine.BaseState GetState(string name);

	public abstract void BindStates();

	public abstract Type GetStateMachineInstanceType();

	public int version { get; protected set; }

	public StateMachine.SerializeType serializable { get; protected set; }

	public virtual void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = null;
	}

	public void InitializeStateMachine()
	{
		this.debugSettings = StateMachineDebuggerSettings.Get().CreateEntry(base.GetType());
		StateMachine.BaseState baseState = null;
		this.InitializeStates(out baseState);
		DebugUtil.Assert(baseState != null);
		this.defaultState = baseState;
	}

	public void CreateStates(object state_machine)
	{
		foreach (FieldInfo fieldInfo in state_machine.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			bool flag = false;
			object[] customAttributes = fieldInfo.GetCustomAttributes(false);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				if (customAttributes[j].GetType() == typeof(StateMachine.DoNotAutoCreate))
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

	public int updateTableSize;

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

	public class BaseDef
	{
		public StateMachine.Instance CreateSMI(IStateMachineTarget master)
		{
			return Singleton<StateMachineManager>.Instance.CreateSMIFromDef(master, this);
		}

		public Type GetStateMachineType()
		{
			return base.GetType().DeclaringType;
		}

		public virtual void Configure(GameObject prefab)
		{
		}

		public bool preventStartSMIOnSpawn;
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
		public abstract StateMachine.BaseState GetCurrentState();

		public abstract void GoTo(StateMachine.BaseState state);

		public abstract float timeinstate { get; }

		public abstract IStateMachineTarget GetMaster();

		public abstract void StopSM(string reason);

		public abstract SchedulerHandle Schedule(float time, Action<object> callback, object callback_data = null);

		public abstract SchedulerHandle ScheduleNextFrame(Action<object> callback, object callback_data = null);

		public virtual void FreeResources()
		{
			this.stateMachine = null;
			if (this.subscribedEvents != null)
			{
				this.subscribedEvents.Clear();
			}
			this.subscribedEvents = null;
			this.parameterContexts = null;
			this.dataTable = null;
			this.updateTable = null;
		}

		public Instance(StateMachine state_machine, IStateMachineTarget master)
		{
			this.stateMachine = state_machine;
			this.CreateParameterContexts();
			this.log = new LoggerFSSSS(this.stateMachine.name, 35);
		}

		public virtual void PostParamsInitialized()
		{
		}

		public bool IsRunning()
		{
			return this.GetCurrentState() != null;
		}

		public void GoTo(string state_name)
		{
			DebugUtil.DevAssert(!KMonoBehaviour.isLoadingScene, "Using Goto while scene was loaded", null);
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

		[Conditional("UNITY_EDITOR")]
		public void Log(string a, string b = "", string c = "", string d = "")
		{
		}

		public bool IsConsoleLoggingEnabled()
		{
			return this.enableConsoleLogging || this.stateMachine.debugSettings.enableConsoleLogging;
		}

		public bool IsBreakOnGoToEnabled()
		{
			return this.breakOnGoTo || this.stateMachine.debugSettings.breakOnGoTo;
		}

		public LoggerFSSSS GetLog()
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
			string text = "";
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
				StateMachineController component = this.GetComponent<StateMachineController>();
				MyAttributes.OnStart(this, component);
				StateMachine.BaseState defaultState = this.stateMachine.GetDefaultState();
				DebugUtil.Assert(defaultState != null);
				if (!component.Restore(this))
				{
					this.PostParamsInitialized();
					this.GoTo(defaultState);
				}
			}
		}

		public bool HasTag(Tag tag)
		{
			return this.GetComponent<KPrefabID>().HasTag(tag);
		}

		public bool IsInsideState(StateMachine.BaseState state)
		{
			StateMachine.BaseState currentState = this.GetCurrentState();
			if (currentState == null)
			{
				return false;
			}
			bool flag = state == currentState;
			int num = 0;
			while (!flag && num < currentState.branch.Length && !(flag = state == currentState.branch[num]))
			{
				num++;
			}
			return flag;
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

		public int Subscribe(int hash, Action<object> handler)
		{
			return this.GetMaster().Subscribe(hash, handler);
		}

		public int Subscribe(int hash, Action<object, object> handler, object context)
		{
			return this.GetMaster().Subscribe(hash, handler, context);
		}

		public void Unsubscribe(int hash, Action<object> handler)
		{
			this.GetMaster().Unsubscribe(hash, handler);
		}

		public void Unsubscribe(int id)
		{
			this.GetMaster().Unsubscribe(id);
		}

		public void Unsubscribe(ref int id)
		{
			this.GetMaster().Unsubscribe(id);
			id = -1;
		}

		public void Trigger(int hash, object data = null)
		{
			this.GetMaster().GetComponent<KPrefabID>().Trigger(hash, data);
		}

		[Obsolete("Use BoxingTrigger to avoid sended boxing object to garbage collection, be careful to unbox parameter in any handlers")]
		public void Trigger<T>(int hash, T data) where T : struct
		{
			this.Trigger(hash, data);
		}

		public void BoxingTrigger(int hash, bool data)
		{
			this.Trigger(hash, BoxedBools.Box(data));
		}

		public void BoxingTrigger<T>(int hash, T data) where T : struct
		{
			Boxed<T> boxed = Boxed<T>.Get(data);
			this.Trigger(hash, boxed);
			boxed.Release();
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

		public string serializationSuffix;

		protected LoggerFSSSS log;

		protected StateMachine.Status status;

		protected StateMachine stateMachine;

		protected Stack<StateEvent.Context> subscribedEvents = new Stack<StateEvent.Context>();

		protected int stackSize;

		protected StateMachine.Parameter.Context[] parameterContexts;

		public object[] dataTable;

		public StateMachine.Instance.UpdateTableEntry[] updateTable;

		private Action<object> scheduleGoToCallback;

		public Action<string, StateMachine.Status> OnStop;

		public bool breakOnGoTo;

		public bool enableConsoleLogging;

		public bool isCrashed;

		public static bool error;

		public struct UpdateTableEntry
		{
			public HandleVector<int>.Handle handle;

			public StateMachineUpdater.BaseUpdateBucket bucket;
		}
	}

	[DebuggerDisplay("{longName}")]
	public class BaseState
	{
		public BaseState()
		{
			this.branch = new StateMachine.BaseState[1];
			this.branch[0] = this;
		}

		public void FreeResources()
		{
			if (this.name == null)
			{
				return;
			}
			this.name = null;
			if (this.defaultState != null)
			{
				this.defaultState.FreeResources();
			}
			this.defaultState = null;
			this.events = null;
			int num = 0;
			while (this.transitions != null && num < this.transitions.Count)
			{
				this.transitions[num].Clear();
				num++;
			}
			this.transitions = null;
			this.enterActions = null;
			this.exitActions = null;
			if (this.branch != null)
			{
				for (int i = 0; i < this.branch.Length; i++)
				{
					this.branch[i].FreeResources();
				}
			}
			this.branch = null;
			this.parent = null;
		}

		public int GetStateCount()
		{
			return this.branch.Length;
		}

		public StateMachine.BaseState GetState(int idx)
		{
			return this.branch[idx];
		}

		public string name;

		public string longName;

		public StateMachine.BaseState defaultState;

		public List<StateEvent> events;

		public List<StateMachine.BaseTransition> transitions;

		public List<StateMachine.UpdateAction> updateActions;

		public List<StateMachine.Action> enterActions;

		public List<StateMachine.Action> exitActions;

		public StateMachine.BaseState[] branch;

		public StateMachine.BaseState parent;
	}

	public class BaseTransition
	{
		public BaseTransition(int idx, string name, StateMachine.BaseState source_state, StateMachine.BaseState target_state)
		{
			this.idx = idx;
			this.name = name;
			this.sourceState = source_state;
			this.targetState = target_state;
		}

		public virtual void Evaluate(StateMachine.Instance smi)
		{
		}

		public virtual StateMachine.BaseTransition.Context Register(StateMachine.Instance smi)
		{
			return new StateMachine.BaseTransition.Context(this);
		}

		public virtual void Unregister(StateMachine.Instance smi, StateMachine.BaseTransition.Context context)
		{
		}

		public void Clear()
		{
			this.name = null;
			if (this.sourceState != null)
			{
				this.sourceState.FreeResources();
			}
			this.sourceState = null;
			if (this.targetState != null)
			{
				this.targetState.FreeResources();
			}
			this.targetState = null;
		}

		public int idx;

		public string name;

		public StateMachine.BaseState sourceState;

		public StateMachine.BaseState targetState;

		public struct Context
		{
			public Context(StateMachine.BaseTransition transition)
			{
				this.idx = transition.idx;
				this.handlerId = 0;
			}

			public int idx;

			public int handlerId;
		}
	}

	public struct UpdateAction
	{
		public int updateTableIdx;

		public UpdateRate updateRate;

		public int nextBucketIdx;

		public StateMachineUpdater.BaseUpdateBucket[] buckets;

		public object updater;
	}

	public struct Action
	{
		public Action(string name, object callback)
		{
			this.name = name;
			this.callback = callback;
		}

		public string name;

		public object callback;
	}

	public class ParameterTransition : StateMachine.BaseTransition
	{
		public ParameterTransition(int idx, string name, StateMachine.BaseState source_state, StateMachine.BaseState target_state)
			: base(idx, name, source_state, target_state)
		{
		}
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

			public abstract void Deserialize(IReader reader, StateMachine.Instance smi);

			public virtual void Cleanup()
			{
			}

			public abstract void ShowEditor(StateMachine.Instance base_smi);

			public abstract void ShowDevTool(StateMachine.Instance base_smi);

			public StateMachine.Parameter parameter;
		}
	}

	public enum SerializeType
	{
		Never,
		ParamsOnly,
		CurrentStateOnly_DEPRECATED,
		Both_DEPRECATED
	}
}
