using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

public class StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine where StateMachineInstanceType : StateMachine.Instance where MasterType : IStateMachineTarget
{
	public override string[] GetStateNames()
	{
		List<string> list = new List<string>();
		foreach (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state in this.states)
		{
			list.Add(state.name);
		}
		return list.ToArray();
	}

	public void Target(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
	{
		this.stateTarget = target;
	}

	public void BindState(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State parent_state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, string state_name)
	{
		if (parent_state != null)
		{
			state_name = parent_state.name + "." + state_name;
		}
		state.name = state_name;
		state.longName = this.name + "." + state_name;
		state.debugPushName = "PuS: " + state.longName;
		state.debugPopName = "PoS: " + state.longName;
		state.debugExecuteName = "EA: " + state.longName;
		List<StateMachine.BaseState> list;
		if (parent_state != null)
		{
			list = new List<StateMachine.BaseState>(parent_state.branch);
		}
		else
		{
			list = new List<StateMachine.BaseState>();
		}
		list.Add(state);
		state.parent = parent_state;
		state.branch = list.ToArray();
		this.maxDepth = Math.Max(state.branch.Length, this.maxDepth);
		this.states.Add(state);
	}

	public void BindStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State parent_state, object state_machine)
	{
		foreach (FieldInfo fieldInfo in state_machine.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (fieldInfo.FieldType.IsSubclassOf(typeof(StateMachine.BaseState)))
			{
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)fieldInfo.GetValue(state_machine);
				string name = fieldInfo.Name;
				this.BindState(parent_state, state, name);
				this.BindStates(state, state);
			}
		}
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.InitializeStates(out default_state);
	}

	public override void BindStates()
	{
		this.BindStates(null, this);
	}

	public override Type GetStateMachineInstanceType()
	{
		return typeof(StateMachineInstanceType);
	}

	public override StateMachine.BaseState GetState(string state_name)
	{
		foreach (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state in this.states)
		{
			if (state.name == state_name)
			{
				return state;
			}
		}
		return null;
	}

	public override void FreeResources()
	{
		for (int i = 0; i < this.states.Count; i++)
		{
			this.states[i].FreeResources();
		}
		this.states.Clear();
		base.FreeResources();
	}

	private List<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State> states = new List<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State>();

	public StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter masterTarget;

	[StateMachine.DoNotAutoCreate]
	protected StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter stateTarget;

	public class GenericInstance : StateMachine.Instance
	{
		public StateMachineType sm { get; private set; }

		protected StateMachineInstanceType smi
		{
			get
			{
				return (StateMachineInstanceType)((object)this);
			}
		}

		public MasterType master { get; private set; }

		public DefType def { get; set; }

		public bool isMasterNull
		{
			get
			{
				return this.internalSm.masterTarget.IsNull((StateMachineInstanceType)((object)this));
			}
		}

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> internalSm
		{
			get
			{
				return (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>)((object)this.sm);
			}
		}

		protected virtual void OnCleanUp()
		{
		}

		public override float timeinstate
		{
			get
			{
				return Time.time - this.stateEnterTime;
			}
		}

		public override void FreeResources()
		{
			this.updateHandle.FreeResources();
			this.updateHandle = default(SchedulerHandle);
			this.controller = null;
			if (this.gotoStack != null)
			{
				this.gotoStack.Clear();
			}
			this.gotoStack = null;
			if (this.transitionStack != null)
			{
				this.transitionStack.Clear();
			}
			this.transitionStack = null;
			if (this.currentSchedulerGroup != null)
			{
				this.currentSchedulerGroup.FreeResources();
			}
			this.currentSchedulerGroup = null;
			if (this.stateStack != null)
			{
				for (int i = 0; i < this.stateStack.Length; i++)
				{
					if (this.stateStack[i].schedulerGroup != null)
					{
						this.stateStack[i].schedulerGroup.FreeResources();
					}
				}
			}
			this.stateStack = null;
			base.FreeResources();
		}

		public GenericInstance(MasterType master)
			: base((StateMachine)((object)Singleton<StateMachineManager>.Instance.CreateStateMachine<StateMachineType>()), master)
		{
			this.master = master;
			this.stateStack = new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance.StackEntry[this.stateMachine.GetMaxDepth()];
			for (int i = 0; i < this.stateStack.Length; i++)
			{
				this.stateStack[i].schedulerGroup = Singleton<StateMachineManager>.Instance.CreateSchedulerGroup();
			}
			this.sm = (StateMachineType)((object)this.stateMachine);
			this.dataTable = new object[base.GetStateMachine().dataTableSize];
			this.updateTable = new StateMachine.Instance.UpdateTableEntry[base.GetStateMachine().updateTableSize];
			this.controller = master.GetComponent<StateMachineController>();
			if (this.controller == null)
			{
				this.controller = master.gameObject.AddComponent<StateMachineController>();
			}
			this.internalSm.masterTarget.Set(master.gameObject, this.smi);
			this.controller.AddStateMachineInstance(this);
		}

		public override IStateMachineTarget GetMaster()
		{
			return this.master;
		}

		private void PushEvent(StateEvent evt)
		{
			StateEvent.Context context = evt.Subscribe(this);
			this.subscribedEvents.Push(context);
		}

		private void PopEvent()
		{
			StateEvent.Context context = this.subscribedEvents.Pop();
			context.stateEvent.Unsubscribe(this, context);
		}

		private void PushTransition(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition transition)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context = transition.Register(this.smi);
			this.transitionStack.Push(context);
		}

		private void PopTransition(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context = this.transitionStack.Pop();
			((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition)state.transitions[context.idx]).Unregister(this.smi, context);
		}

		private void PushState(StateMachine.BaseState state)
		{
			int num = this.gotoId;
			this.currentActionIdx = -1;
			if (state.events != null)
			{
				foreach (StateEvent stateEvent in state.events)
				{
					this.PushEvent(stateEvent);
				}
			}
			if (state.transitions != null)
			{
				foreach (StateMachine.BaseTransition baseTransition in state.transitions)
				{
					StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition transition = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition)baseTransition;
					this.PushTransition(transition);
				}
			}
			if (state.parameterTransitions != null)
			{
				foreach (StateMachine.ParameterTransition parameterTransition in state.parameterTransitions)
				{
					((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition)parameterTransition).Register(this.smi);
				}
			}
			if (state.updateActions != null)
			{
				for (int i = 0; i < state.updateActions.Count; i++)
				{
					StateMachine.UpdateAction updateAction = state.updateActions[i];
					int updateTableIdx = updateAction.updateTableIdx;
					int nextBucketIdx = updateAction.nextBucketIdx;
					updateAction.nextBucketIdx = (updateAction.nextBucketIdx + 1) % updateAction.buckets.Length;
					UpdateBucketWithUpdater<StateMachineInstanceType> updateBucketWithUpdater = (UpdateBucketWithUpdater<StateMachineInstanceType>)updateAction.buckets[nextBucketIdx];
					this.smi.updateTable[updateTableIdx].bucket = updateBucketWithUpdater;
					this.smi.updateTable[updateTableIdx].handle = updateBucketWithUpdater.Add(this.smi, Singleton<StateMachineUpdater>.Instance.GetFrameTime(updateAction.updateRate, updateBucketWithUpdater.frame), (UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater)updateAction.updater);
					state.updateActions[i] = updateAction;
				}
			}
			this.stateEnterTime = Time.time;
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance.StackEntry[] array = this.stateStack;
			int stackSize = this.stackSize;
			this.stackSize = stackSize + 1;
			array[stackSize].state = state;
			this.currentSchedulerGroup = this.stateStack[this.stackSize - 1].schedulerGroup;
			if (state.transitions != null)
			{
				foreach (StateMachine.BaseTransition baseTransition2 in state.transitions)
				{
					StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition transition2 = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition)baseTransition2;
					if (num != this.gotoId)
					{
						return;
					}
					transition2.Evaluate(this.smi);
				}
			}
			if (state.parameterTransitions != null)
			{
				foreach (StateMachine.ParameterTransition parameterTransition2 in state.parameterTransitions)
				{
					StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition parameterTransition3 = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition)parameterTransition2;
					if (num != this.gotoId)
					{
						return;
					}
					parameterTransition3.Evaluate(this.smi);
				}
			}
			if (num != this.gotoId)
			{
				return;
			}
			this.ExecuteActions((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)state, state.enterActions);
			int num2 = this.gotoId;
		}

		private void ExecuteActions(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, List<StateMachine.Action> actions)
		{
			if (actions == null)
			{
				return;
			}
			int num = this.gotoId;
			this.currentActionIdx++;
			while (this.currentActionIdx < actions.Count && num == this.gotoId)
			{
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback)actions[this.currentActionIdx].callback;
				try
				{
					callback(this.smi);
				}
				catch (Exception ex)
				{
					if (!StateMachine.Instance.error)
					{
						base.Error();
						string text = "(NULL).";
						IStateMachineTarget master = this.GetMaster();
						if (!master.isNull)
						{
							KPrefabID component = master.GetComponent<KPrefabID>();
							if (component != null)
							{
								text = "(" + component.PrefabTag.ToString() + ").";
							}
							else
							{
								text = "(" + base.gameObject.name + ").";
							}
						}
						string text2 = string.Concat(new string[]
						{
							"Exception in: ",
							text,
							this.stateMachine.ToString(),
							".",
							state.name,
							".",
							actions[this.currentActionIdx].name
						});
						DebugUtil.LogErrorArgs(this.controller, new object[] { text2 + "\n" + ex.ToString() });
					}
				}
				this.currentActionIdx++;
			}
			this.currentActionIdx = 2147483646;
		}

		private void PopState()
		{
			this.currentActionIdx = -1;
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance.StackEntry[] array = this.stateStack;
			int num = this.stackSize - 1;
			this.stackSize = num;
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance.StackEntry stackEntry = array[num];
			StateMachine.BaseState state = stackEntry.state;
			if (state.parameterTransitions != null)
			{
				foreach (StateMachine.ParameterTransition parameterTransition in state.parameterTransitions)
				{
					((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition)parameterTransition).Unregister(this.smi);
				}
			}
			if (state.transitions != null)
			{
				for (int i = 0; i < state.transitions.Count; i++)
				{
					this.PopTransition((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)state);
				}
			}
			if (state.events != null)
			{
				for (int j = 0; j < state.events.Count; j++)
				{
					this.PopEvent();
				}
			}
			if (state.updateActions != null)
			{
				foreach (StateMachine.UpdateAction updateAction in state.updateActions)
				{
					int updateTableIdx = updateAction.updateTableIdx;
					StateMachineUpdater.BaseUpdateBucket baseUpdateBucket = (UpdateBucketWithUpdater<StateMachineInstanceType>)this.smi.updateTable[updateTableIdx].bucket;
					this.smi.updateTable[updateTableIdx].bucket = null;
					baseUpdateBucket.Remove(this.smi.updateTable[updateTableIdx].handle);
				}
			}
			stackEntry.schedulerGroup.Reset();
			this.currentSchedulerGroup = stackEntry.schedulerGroup;
			this.ExecuteActions((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)state, state.exitActions);
		}

		public override SchedulerHandle Schedule(float time, Action<object> callback, object callback_data = null)
		{
			return Singleton<StateMachineManager>.Instance.Schedule(this.GetCurrentState().longName, time, callback, callback_data, this.currentSchedulerGroup);
		}

		public override void StartSM()
		{
			if (this.controller != null && !this.controller.HasStateMachineInstance(this))
			{
				this.controller.AddStateMachineInstance(this);
			}
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			if (StateMachine.Instance.error)
			{
				return;
			}
			if (this.controller != null)
			{
				this.controller.RemoveStateMachineInstance(this);
			}
			if (!base.IsRunning())
			{
				return;
			}
			this.gotoId++;
			while (this.stackSize > 0)
			{
				this.PopState();
			}
			if (this.master != null && this.controller != null)
			{
				this.controller.RemoveStateMachineInstance(this);
			}
			if (this.status == StateMachine.Status.Running)
			{
				base.SetStatus(StateMachine.Status.Failed);
			}
			if (this.OnStop != null)
			{
				this.OnStop(reason, this.status);
			}
			for (int i = 0; i < this.parameterContexts.Length; i++)
			{
				this.parameterContexts[i].Cleanup();
			}
			this.OnCleanUp();
		}

		private void FinishStateInProgress(StateMachine.BaseState state)
		{
			if (state.enterActions == null)
			{
				return;
			}
			this.ExecuteActions((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)state, state.enterActions);
		}

		public override void GoTo(StateMachine.BaseState base_state)
		{
			if (App.IsExiting)
			{
				return;
			}
			if (StateMachine.Instance.error)
			{
				return;
			}
			if (this.isMasterNull)
			{
				return;
			}
			try
			{
				if (base.IsBreakOnGoToEnabled())
				{
					Debugger.Break();
				}
				if (base_state != null)
				{
					while (base_state.defaultState != null)
					{
						base_state = base_state.defaultState;
					}
				}
				if (this.GetCurrentState() == null)
				{
					base.SetStatus(StateMachine.Status.Running);
				}
				if (this.gotoStack.Count > 100)
				{
					string text = "Potential infinite transition loop detected in state machine: " + this.ToString() + "\nGoto stack:\n";
					foreach (StateMachine.BaseState baseState in this.gotoStack)
					{
						text = text + "\n" + baseState.name;
					}
					global::Debug.LogError(text);
					base.Error();
				}
				else
				{
					this.gotoStack.Push(base_state);
					if (base_state == null)
					{
						this.StopSM("StateMachine.GoTo(null)");
						this.gotoStack.Pop();
					}
					else
					{
						int num = this.gotoId + 1;
						this.gotoId = num;
						int num2 = num;
						StateMachine.BaseState[] branch = (base_state as StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State).branch;
						int num3 = 0;
						while (num3 < this.stackSize && num3 < branch.Length && this.stateStack[num3].state == branch[num3])
						{
							num3++;
						}
						int num4 = this.stackSize - 1;
						if (num4 >= 0 && num4 == num3 - 1)
						{
							this.FinishStateInProgress(this.stateStack[num4].state);
						}
						while (this.stackSize > num3 && num2 == this.gotoId)
						{
							this.PopState();
						}
						int num5 = num3;
						while (num5 < branch.Length && num2 == this.gotoId)
						{
							this.PushState(branch[num5]);
							num5++;
						}
						this.gotoStack.Pop();
					}
				}
			}
			catch (Exception ex)
			{
				if (!StateMachine.Instance.error)
				{
					base.Error();
					string text2 = "(Stop)";
					if (base_state != null)
					{
						text2 = base_state.name;
					}
					string text3 = "(NULL).";
					if (!this.GetMaster().isNull)
					{
						text3 = "(" + base.gameObject.name + ").";
					}
					string text4 = string.Concat(new string[]
					{
						"Exception in: ",
						text3,
						this.stateMachine.ToString(),
						".GoTo(",
						text2,
						")"
					});
					DebugUtil.LogErrorArgs(this.controller, new object[] { text4 + "\n" + ex.ToString() });
				}
			}
		}

		public override StateMachine.BaseState GetCurrentState()
		{
			if (this.stackSize > 0)
			{
				return this.stateStack[this.stackSize - 1].state;
			}
			return null;
		}

		private float stateEnterTime;

		private int gotoId;

		private int currentActionIdx = -1;

		private SchedulerHandle updateHandle;

		private Stack<StateMachine.BaseState> gotoStack = new Stack<StateMachine.BaseState>();

		protected Stack<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context> transitionStack = new Stack<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context>();

		protected StateMachineController controller;

		private SchedulerGroup currentSchedulerGroup;

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance.StackEntry[] stateStack;

		public struct StackEntry
		{
			public StateMachine.BaseState state;

			public SchedulerGroup schedulerGroup;
		}
	}

	public class State : StateMachine.BaseState
	{
		protected StateMachineType sm;

		public delegate void Callback(StateMachineInstanceType smi);
	}

	public new abstract class ParameterTransition : StateMachine.ParameterTransition
	{
		public abstract void Evaluate(StateMachineInstanceType smi);

		public abstract void Register(StateMachineInstanceType smi);

		public abstract void Unregister(StateMachineInstanceType smi);
	}

	public class Transition : StateMachine.BaseTransition
	{
		public Transition(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State source_state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state, int idx, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition)
			: base(name, source_state, target_state)
		{
			this.condition = condition;
			this.idx = idx;
		}

		public virtual void Evaluate(StateMachineInstanceType smi)
		{
		}

		public virtual StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context Register(StateMachineInstanceType smi)
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context(this);
		}

		public virtual void Unregister(StateMachineInstanceType smi, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context)
		{
		}

		public StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition;

		public int idx;

		public delegate bool ConditionCallback(StateMachineInstanceType smi);

		public struct Context
		{
			public Context(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition transition)
			{
				this.idx = transition.idx;
				this.handlerId = 0;
			}

			public int idx;

			public int handlerId;
		}
	}

	public abstract class Parameter<ParameterType> : StateMachine.Parameter
	{
		public Parameter()
		{
		}

		public Parameter(ParameterType default_value)
		{
			this.defaultValue = default_value;
		}

		public ParameterType Set(ParameterType value, StateMachineInstanceType smi)
		{
			((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context)smi.GetParameterContext(this)).Set(value, smi);
			return value;
		}

		public ParameterType Get(StateMachineInstanceType smi)
		{
			return ((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context)smi.GetParameterContext(this)).value;
		}

		public ParameterType defaultValue;

		public bool isSignal;

		public delegate bool Callback(StateMachineInstanceType smi, ParameterType p);

		public class Transition : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition
		{
			public Transition(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType> parameter, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Callback callback)
			{
				this.parameter = parameter;
				this.callback = callback;
				this.state = state;
			}

			public override void Evaluate(StateMachineInstanceType smi)
			{
				if (this.parameter.isSignal && this.callback == null)
				{
					return;
				}
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context)smi.GetParameterContext(this.parameter);
				if (this.callback(smi, context.value))
				{
					smi.GoTo(this.state);
				}
			}

			private void Trigger(StateMachineInstanceType smi)
			{
				smi.GoTo(this.state);
			}

			public override void Register(StateMachineInstanceType smi)
			{
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context)smi.GetParameterContext(this.parameter);
				if (this.parameter.isSignal && this.callback == null)
				{
					StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context2 = context;
					context2.onDirty = (Action<StateMachineInstanceType>)Delegate.Combine(context2.onDirty, new Action<StateMachineInstanceType>(this.Trigger));
					return;
				}
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context3 = context;
				context3.onDirty = (Action<StateMachineInstanceType>)Delegate.Combine(context3.onDirty, new Action<StateMachineInstanceType>(this.Evaluate));
			}

			public override void Unregister(StateMachineInstanceType smi)
			{
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context)smi.GetParameterContext(this.parameter);
				if (this.parameter.isSignal && this.callback == null)
				{
					StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context2 = context;
					context2.onDirty = (Action<StateMachineInstanceType>)Delegate.Remove(context2.onDirty, new Action<StateMachineInstanceType>(this.Trigger));
					return;
				}
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Context context3 = context;
				context3.onDirty = (Action<StateMachineInstanceType>)Delegate.Remove(context3.onDirty, new Action<StateMachineInstanceType>(this.Evaluate));
			}

			public override string ToString()
			{
				if (this.state != null)
				{
					return this.parameter.name + "->" + this.state.name;
				}
				return this.parameter.name + "->(Stop)";
			}

			private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType> parameter;

			private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Callback callback;

			private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state;
		}

		public new abstract class Context : StateMachine.Parameter.Context
		{
			public Context(StateMachine.Parameter parameter, ParameterType default_value)
				: base(parameter)
			{
				this.value = default_value;
			}

			public virtual void Set(ParameterType value, StateMachineInstanceType smi)
			{
				if (!EqualityComparer<ParameterType>.Default.Equals(value, this.value))
				{
					this.value = value;
					if (this.onDirty != null)
					{
						this.onDirty(smi);
					}
				}
			}

			public ParameterType value;

			public Action<StateMachineInstanceType> onDirty;
		}
	}

	public class BoolParameter : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>
	{
		public BoolParameter()
		{
		}

		public BoolParameter(bool default_value)
			: base(default_value)
		{
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.BoolParameter.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>.Context
		{
			public Context(StateMachine.Parameter parameter, bool default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(this.value ? 1 : 0);
			}

			public override void Deserialize(IReader reader)
			{
				this.value = reader.ReadByte() > 0;
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class Vector3Parameter : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<Vector3>
	{
		public Vector3Parameter()
		{
		}

		public Vector3Parameter(Vector3 default_value)
			: base(default_value)
		{
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Vector3Parameter.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<Vector3>.Context
		{
			public Context(StateMachine.Parameter parameter, Vector3 default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(this.value.x);
				writer.Write(this.value.y);
				writer.Write(this.value.z);
			}

			public override void Deserialize(IReader reader)
			{
				this.value.x = reader.ReadSingle();
				this.value.y = reader.ReadSingle();
				this.value.z = reader.ReadSingle();
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class EnumParameter<EnumType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<EnumType>
	{
		public EnumParameter(EnumType default_value)
			: base(default_value)
		{
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.EnumParameter<EnumType>.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<EnumType>.Context
		{
			public Context(StateMachine.Parameter parameter, EnumType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write((int)((object)this.value));
			}

			public override void Deserialize(IReader reader)
			{
				this.value = (EnumType)((object)reader.ReadInt32());
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class FloatParameter : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>
	{
		public FloatParameter()
		{
		}

		public FloatParameter(float default_value)
			: base(default_value)
		{
		}

		public float Delta(float delta_value, StateMachineInstanceType smi)
		{
			float num = base.Get(smi);
			num += delta_value;
			base.Set(num, smi);
			return num;
		}

		public float DeltaClamp(float delta_value, float min_value, float max_value, StateMachineInstanceType smi)
		{
			float num = base.Get(smi);
			num += delta_value;
			num = Mathf.Clamp(num, min_value, max_value);
			base.Set(num, smi);
			return num;
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Context
		{
			public Context(StateMachine.Parameter parameter, float default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(this.value);
			}

			public override void Deserialize(IReader reader)
			{
				this.value = reader.ReadSingle();
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class IntParameter : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<int>
	{
		public IntParameter()
		{
		}

		public IntParameter(int default_value)
			: base(default_value)
		{
		}

		public int Delta(int delta_value, StateMachineInstanceType smi)
		{
			int num = base.Get(smi);
			num += delta_value;
			base.Set(num, smi);
			return num;
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.IntParameter.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<int>.Context
		{
			public Context(StateMachine.Parameter parameter, int default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				writer.Write(this.value);
			}

			public override void Deserialize(IReader reader)
			{
				this.value = reader.ReadInt32();
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class ResourceParameter<ResourceType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ResourceType> where ResourceType : Resource
	{
		public ResourceParameter()
			: base(default(ResourceType))
		{
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ResourceParameter<ResourceType>.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ResourceType>.Context
		{
			public Context(StateMachine.Parameter parameter, ResourceType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
				string text = "";
				if (this.value != null)
				{
					if (this.value.Guid == null)
					{
						global::Debug.LogError("Cannot serialize resource with invalid guid: " + this.value.Id);
					}
					else
					{
						text = this.value.Guid.Guid;
					}
				}
				writer.WriteKleiString(text);
			}

			public override void Deserialize(IReader reader)
			{
				string text = reader.ReadKleiString();
				if (text != "")
				{
					ResourceGuid resourceGuid = new ResourceGuid(text, null);
					this.value = Db.Get().GetResource<ResourceType>(resourceGuid);
				}
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class ObjectParameter<ObjectType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ObjectType> where ObjectType : class
	{
		public ObjectParameter()
			: base(default(ObjectType))
		{
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ObjectParameter<ObjectType>.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ObjectType>.Context
		{
			public Context(StateMachine.Parameter parameter, ObjectType default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}

	public class TargetParameter : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>
	{
		public TargetParameter()
			: base(null)
		{
		}

		public SMT GetSMI<SMT>(StateMachineInstanceType smi) where SMT : StateMachine.Instance
		{
			GameObject gameObject = base.Get(smi);
			if (gameObject != null)
			{
				SMT smi2 = gameObject.GetSMI<SMT>();
				if (smi2 != null)
				{
					return smi2;
				}
				global::Debug.LogError(gameObject.name + " does not have state machine " + typeof(StateMachineType).Name);
			}
			return default(SMT);
		}

		public bool IsNull(StateMachineInstanceType smi)
		{
			return base.Get(smi) == null;
		}

		public ComponentType Get<ComponentType>(StateMachineInstanceType smi)
		{
			GameObject gameObject = base.Get(smi);
			if (gameObject != null)
			{
				ComponentType component = gameObject.GetComponent<ComponentType>();
				if (component != null)
				{
					return component;
				}
				global::Debug.LogError(gameObject.name + " does not have component " + typeof(ComponentType).Name);
			}
			return default(ComponentType);
		}

		public void Set(KMonoBehaviour value, StateMachineInstanceType smi)
		{
			GameObject gameObject = null;
			if (value != null)
			{
				gameObject = value.gameObject;
			}
			base.Set(gameObject, smi);
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Context
		{
			public Context(StateMachine.Parameter parameter, GameObject default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void Cleanup()
			{
				base.Cleanup();
				if (this.value != null)
				{
					this.value.GetComponent<KMonoBehaviour>().Unsubscribe(this.objectDestroyedHandler);
					this.objectDestroyedHandler = 0;
				}
			}

			public override void Set(GameObject value, StateMachineInstanceType smi)
			{
				this.smi = smi;
				if (this.value != null)
				{
					this.value.GetComponent<KMonoBehaviour>().Unsubscribe(this.objectDestroyedHandler);
					this.objectDestroyedHandler = 0;
				}
				if (value != null)
				{
					this.objectDestroyedHandler = value.GetComponent<KMonoBehaviour>().Subscribe(1969584890, new Action<object>(this.OnObjectDestroyed));
				}
				base.Set(value, smi);
			}

			private void OnObjectDestroyed(object data)
			{
				this.Set(null, this.smi);
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}

			private StateMachineInstanceType smi;

			private int objectDestroyedHandler;
		}
	}

	public class SignalParameter
	{
	}

	public class Signal : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>
	{
		public Signal()
			: base(null)
		{
			this.isSignal = true;
		}

		public void Trigger(StateMachineInstanceType smi)
		{
			((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal.Context)smi.GetParameterContext(this)).Set(null, smi);
		}

		public override StateMachine.Parameter.Context CreateContext()
		{
			return new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal.Context(this, this.defaultValue);
		}

		public new class Context : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>.Context
		{
			public Context(StateMachine.Parameter parameter, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter default_value)
				: base(parameter, default_value)
			{
			}

			public override void Serialize(BinaryWriter writer)
			{
			}

			public override void Deserialize(IReader reader)
			{
			}

			public override void Set(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter value, StateMachineInstanceType smi)
			{
				if (this.onDirty != null)
				{
					this.onDirty(smi);
				}
			}

			public override void ShowEditor(StateMachine.Instance base_smi)
			{
			}
		}
	}
}
