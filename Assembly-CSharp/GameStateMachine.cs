using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance where MasterType : IStateMachineTarget
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.InitializeStates(out default_state);
	}

	public override void BindStates()
	{
		base.BindState(null, this.root, "root");
		base.BindStates(this.root, this);
	}

	public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State root = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State();

	public class PLPState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pre;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State loop;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pst;
	}

	public class GameInstance : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance
	{
		public GameInstance(MasterType master, DefType def)
			: base(master)
		{
			base.def = def;
			this.animController = master.GetComponent<KAnimControllerBase>();
		}

		public GameInstance(MasterType master)
			: base(master)
		{
			this.animController = master.GetComponent<KAnimControllerBase>();
		}

		public void Queue(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			if (this.animController != null)
			{
				this.animController.Queue(anim, mode, 1f, 0f);
			}
			else
			{
				MasterType master = base.master;
				global::Debug.LogWarning(master.name + " is missing a anim controller", null);
			}
		}

		public void Play(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			if (this.animController != null)
			{
				this.animController.Play(anim, mode, 1f, 0f);
			}
			else
			{
				MasterType master = base.master;
				global::Debug.LogWarning(master.name + " is missing a anim controller", null);
			}
		}

		public KAnimControllerBase animController;
	}

	public class TagTransitionData : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition
	{
		public TagTransitionData(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State source_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state, int idx, Tag[] tags, bool on_remove, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
			: base(tags.ToString(), source_state, target_state, idx, null)
		{
			this.tags = tags;
			this.onRemove = on_remove;
			this.target = target;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (!this.onRemove)
			{
				if (!this.HasAllTags(smi))
				{
					return;
				}
			}
			else
			{
				bool flag = this.HasAnyTags(smi);
				if (flag)
				{
					return;
				}
			}
			this.ExecuteTransition(smi);
		}

		private bool HasAllTags(StateMachineInstanceType smi)
		{
			KPrefabID component = this.target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < this.tags.Length; i++)
			{
				if (!component.HasTag(this.tags[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool HasAnyTags(StateMachineInstanceType smi)
		{
			KPrefabID component = this.target.Get(smi).GetComponent<KPrefabID>();
			for (int i = 0; i < this.tags.Length; i++)
			{
				if (component.HasTag(this.tags[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			if (this.targetState != null)
			{
				string name = this.targetState.name;
			}
			smi.GoTo(this.targetState);
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (!this.onRemove)
			{
				if (!this.HasAllTags(smi))
				{
					return;
				}
			}
			else
			{
				bool flag = this.HasAnyTags(smi);
				if (flag)
				{
					return;
				}
			}
			this.ExecuteTransition(smi);
		}

		public override StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context Register(StateMachineInstanceType smi)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context = base.Register(smi);
			context.handlerId = this.target.Get(smi).Subscribe(-1582839653, delegate(object data)
			{
				this.OnCallback(smi);
			});
			return context;
		}

		public override void Unregister(StateMachineInstanceType smi, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context)
		{
			base.Unregister(smi, context);
			this.target.Get(smi).Unsubscribe(context.handlerId);
		}

		private Tag[] tags;

		private bool onRemove;

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;
	}

	public class EventTransitionData : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition
	{
		public EventTransitionData(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State source_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state, int idx, GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
			: base(evt.ToString(), source_state, target_state, idx, condition)
		{
			this.evtId = evt;
			this.eventName = evt.ToString();
			this.target = target;
			this.globalEventSystemCallback = global_event_system_callback;
		}

		public override void Evaluate(StateMachineInstanceType smi)
		{
			if (this.condition != null && this.condition(smi))
			{
				this.ExecuteTransition(smi);
			}
		}

		private void ExecuteTransition(StateMachineInstanceType smi)
		{
			if (this.targetState != null)
			{
				string name = this.targetState.name;
			}
			smi.GoTo(this.targetState);
		}

		private void OnCallback(StateMachineInstanceType smi)
		{
			if (this.condition == null || this.condition(smi))
			{
				this.ExecuteTransition(smi);
			}
		}

		public override StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context Register(StateMachineInstanceType smi)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context = base.Register(smi);
			Action<object> action = delegate(object d)
			{
				this.OnCallback(smi);
			};
			GameObject gameObject;
			if (this.globalEventSystemCallback != null)
			{
				gameObject = this.globalEventSystemCallback(smi).gameObject;
			}
			else
			{
				gameObject = this.target.Get(smi);
				if (gameObject == null)
				{
					throw new InvalidOperationException("TargetParameter: " + this.target.name + " is null");
				}
			}
			context.handlerId = gameObject.Subscribe((int)this.evtId, action);
			return context;
		}

		public override void Unregister(StateMachineInstanceType smi, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.Context context)
		{
			base.Unregister(smi, context);
			GameObject gameObject = null;
			if (this.globalEventSystemCallback != null)
			{
				KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(smi);
				if (kmonoBehaviour != null)
				{
					gameObject = kmonoBehaviour.gameObject;
				}
			}
			else
			{
				gameObject = this.target.Get(smi);
			}
			if (gameObject != null)
			{
				gameObject.Unsubscribe(context.handlerId);
			}
		}

		private GameHashes evtId;

		private string eventName;

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;
	}

	public new class State : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter GetStateTarget()
		{
			if (this.stateTarget != null)
			{
				return this.stateTarget;
			}
			if (this.parent != null)
			{
				GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state = (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State)this.parent;
				return state.GetStateTarget();
			}
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter targetParameter = this.sm.stateTarget;
			if (targetParameter == null)
			{
				return this.sm.masterTarget;
			}
			return targetParameter;
		}

		public int CreateDataTableEntry()
		{
			return this.sm.dataTableSize++;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State root
		{
			get
			{
				return this;
			}
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoNothing()
		{
			return this;
		}

		private static StateMachine.Action[] AddAction(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback, StateMachine.Action[] actions, bool add_to_end)
		{
			List<StateMachine.Action> list;
			if (actions != null)
			{
				list = new List<StateMachine.Action>(actions);
			}
			else
			{
				list = new List<StateMachine.Action>();
			}
			StateMachine.Action action = new StateMachine.Action(name, name, callback);
			if (add_to_end)
			{
				list.Add(action);
			}
			else
			{
				list.Insert(0, action);
			}
			return list.ToArray();
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State master
		{
			get
			{
				this.stateTarget = this.sm.masterTarget;
				return this;
			}
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Target(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
		{
			this.stateTarget = target;
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Update(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			return this.Update("Update", callback);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Enter(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			return this.Enter("Enter", callback);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Exit(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			return this.Exit("Exit", callback);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Update(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			this.updateActions = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.AddAction(name, callback, this.updateActions, true);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Enter(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			this.enterActions = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.AddAction(name, callback, this.enterActions, true);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Exit(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			this.exitActions = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.AddAction(name, callback, this.exitActions, false);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Toggle(string name, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback enter_callback, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback exit_callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("ToggleEnter(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.HasToggleEnteredFlag;
				enter_callback(smi);
			});
			this.Exit("ToggleExit(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					smi.dataTable[data_idx] = null;
					exit_callback(smi);
				}
			});
			return this;
		}

		private void Break(StateMachineInstanceType smi)
		{
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BreakOnEnter()
		{
			return this.Enter(delegate(StateMachineInstanceType smi)
			{
				this.Break(smi);
			});
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BreakOnExit()
		{
			return this.Exit(delegate(StateMachineInstanceType smi)
			{
				this.Break(smi);
			});
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State AddEffect(string effect_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, true);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAnims(Func<StateMachineInstanceType, HashedString> chooser_callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableAnims()", delegate(StateMachineInstanceType smi)
			{
				HashedString hashedString = chooser_callback(smi);
				if (hashedString.IsValid())
				{
					KAnimFile anim = Assets.GetAnim(hashedString);
					if (anim == null)
					{
						global::Debug.LogWarning("Missing anims: " + hashedString, null);
					}
					else
					{
						state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim, 0f);
					}
				}
			});
			this.Exit("Disableanims()", delegate(StateMachineInstanceType smi)
			{
				HashedString hashedString2 = chooser_callback(smi);
				if (hashedString2.IsValid())
				{
					KAnimFile anim2 = Assets.GetAnim(hashedString2);
					if (anim2 != null)
					{
						state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim2);
					}
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleHideSymbol(string symbol_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			KAnimHashedString symbol_hash = new KAnimHashedString(symbol_name);
			this.Enter("HideSymbol(" + symbol_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KAnimControllerBase>(smi).HideSymbol(symbol_hash, true);
			});
			this.Exit("ShowSymbol(" + symbol_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KAnimControllerBase>(smi).ShowSymbol(symbol_hash);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Tag(params Tag[] tags)
		{
			this.tags = tags;
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAnims(string anim_file, float priority = 0f)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableAnims(" + anim_file + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimFile anim = Assets.GetAnim(anim_file);
				if (anim == null)
				{
					global::Debug.LogWarning("Missing anims: " + anim_file, null);
				}
				else
				{
					KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
					kanimControllerBase.RemoveAnimOverrides(anim);
					kanimControllerBase.AddAnimOverrides(anim, priority);
				}
			});
			this.Exit("Disableanims(" + anim_file + ")", delegate(StateMachineInstanceType smi)
			{
				KAnimFile anim2 = Assets.GetAnim(anim_file);
				if (anim2 != null)
				{
					state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim2);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAttributeModifier(string modifier_name, Func<StateMachineInstanceType, AttributeModifier> callback, Func<StateMachineInstanceType, bool> condition = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					AttributeModifier attributeModifier = callback(smi);
					DebugUtil.Assert(smi.dataTable[data_idx] == null, "Assert!");
					smi.dataTable[data_idx] = attributeModifier;
					state_target.Get(smi).GetAttributes().Add(modifier_name, attributeModifier);
				}
			});
			this.Exit("RemoveAttributeModifier( " + modifier_name + " )", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] != null)
				{
					AttributeModifier attributeModifier2 = (AttributeModifier)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					GameObject gameObject = state_target.Get(smi);
					if (gameObject != null)
					{
						gameObject.GetAttributes().Remove(attributeModifier2);
					}
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State WorkableStartTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("Enter WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if (workable != null)
				{
					global::System.Action action = delegate
					{
						smi.GoTo(target_state);
					};
					smi.dataTable[data_idx] = action;
					Workable workable2 = workable;
					workable2.OnWorkStartedCB = (global::System.Action)Delegate.Combine(workable2.OnWorkStartedCB, action);
				}
			});
			this.Exit("Exit WorkableStartTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable3 = get_workable_callback(smi);
				if (workable3 != null)
				{
					global::System.Action action2 = (global::System.Action)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					Workable workable4 = workable3;
					workable4.OnWorkStartedCB = (global::System.Action)Delegate.Remove(workable4.OnWorkStartedCB, action2);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State WorkableStopTransition(Func<StateMachineInstanceType, Workable> get_workable_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("Enter WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable = get_workable_callback(smi);
				if (workable != null)
				{
					global::System.Action action = delegate
					{
						smi.GoTo(target_state);
					};
					smi.dataTable[data_idx] = action;
					Workable workable2 = workable;
					workable2.OnWorkStoppedCB = (global::System.Action)Delegate.Combine(workable2.OnWorkStoppedCB, action);
				}
			});
			this.Exit("Exit WorkableStopTransition(" + target_state.longName + ")", delegate(StateMachineInstanceType smi)
			{
				Workable workable3 = get_workable_callback(smi);
				if (workable3 != null)
				{
					global::System.Action action2 = (global::System.Action)smi.dataTable[data_idx];
					smi.dataTable[data_idx] = null;
					Workable workable4 = workable3;
					workable4.OnWorkStoppedCB = (global::System.Action)Delegate.Remove(workable4.OnWorkStoppedCB, action2);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleNavigationAbilities(PathFinderFlags flags)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableNavigationAbilities(" + flags.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Navigator>(smi).SetAbilityFlag(flags);
			});
			this.Exit("DisableNavigationFlags(" + flags.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Navigator>(smi).ClearAbilityFlag(flags);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleGravity()
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				smi.dataTable[data_idx] = gameObject;
				GameComps.Gravities.Add(gameObject, Vector2.zero, null);
			});
			this.Exit("RemoveComponent<Gravity>()", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject2 = (GameObject)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				GameComps.Gravities.Remove(gameObject2);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleGravity(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State landed_state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.EventTransition(GameHashes.Landed, landed_state, null);
			this.Toggle("GravityComponent", delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Add(state_target.Get(smi), Vector2.zero, null);
			}, delegate(StateMachineInstanceType smi)
			{
				GameComps.Gravities.Remove(state_target.Get(smi));
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleThought(Func<StateMachineInstanceType, Thought> chooser_callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought = chooser_callback(smi);
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
			});
			this.Exit("DisableThought()", delegate(StateMachineInstanceType smi)
			{
				Thought thought2 = chooser_callback(smi);
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought2);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleThought(Thought thought, Func<StateMachineInstanceType, bool> condition_callback = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition_callback == null || condition_callback(smi))
				{
					state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
				}
			});
			if (condition_callback != null)
			{
				this.Update("ValidateThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
				{
					if (condition_callback(smi))
					{
						state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
					}
					else
					{
						state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
					}
				});
			}
			this.Exit("RemoveThought(" + thought.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleExpression(Func<StateMachineInstanceType, Expression> chooser_callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).AddExpression(chooser_callback(smi));
			});
			this.Exit("RemoveExpression", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<FaceGraph>(smi).RemoveExpression(chooser_callback(smi));
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleExpression(Expression expression, Func<StateMachineInstanceType, bool> condition = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition == null || condition(smi))
				{
					state_target.Get<FaceGraph>(smi).AddExpression(expression);
				}
			});
			if (condition != null)
			{
				this.Update("ValidateExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
				{
					if (condition(smi))
					{
						state_target.Get<FaceGraph>(smi).AddExpression(expression);
					}
					else
					{
						state_target.Get<FaceGraph>(smi).RemoveExpression(expression);
					}
				});
			}
			this.Exit("RemoveExpression(" + expression.Id + ")", delegate(StateMachineInstanceType smi)
			{
				FaceGraph faceGraph = state_target.Get<FaceGraph>(smi);
				if (faceGraph != null)
				{
					faceGraph.RemoveExpression(expression);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleMainStatusItem(StatusItem status_item)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KSelectable>(smi).SetStatusItem(Db.Get().StatusItemCategories.Main, status_item, smi);
			});
			this.Exit("RemoveMainStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kselectable = state_target.Get<KSelectable>(smi);
				if (kselectable != null)
				{
					kselectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleCategoryStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter(string.Concat(new string[] { "AddCategoryStatusItem(", category.Id, ", ", status_item.Id, ")" }), delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, (data == null) ? smi : data);
			});
			this.Exit(string.Concat(new string[] { "RemoveCategoryStatusItem(", category.Id, ", ", status_item.Id, ")" }), delegate(StateMachineInstanceType smi)
			{
				KSelectable kselectable = state_target.Get<KSelectable>(smi);
				if (kselectable != null)
				{
					kselectable.SetStatusItem(category, null, null);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(StatusItem status_item, object data = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				object obj = data;
				if (obj == null)
				{
					obj = smi;
				}
				Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, obj);
				smi.dataTable[data_idx] = guid;
			});
			this.Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kselectable = state_target.Get<KSelectable>(smi);
				if (kselectable != null && smi.dataTable[data_idx] != null)
				{
					Guid guid2 = (Guid)smi.dataTable[data_idx];
					kselectable.RemoveStatusItem(guid2, false);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleSnapOn(string snap_on)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("SnapOn(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<SnapOn>(smi).AttachSnapOnByName(snap_on);
			});
			this.Exit("SnapOff(" + snap_on + ")", delegate(StateMachineInstanceType smi)
			{
				SnapOn snapOn = state_target.Get<SnapOn>(smi);
				if (snapOn != null)
				{
					snapOn.DetachSnapOnByName(snap_on);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleTag(Tag tag)
		{
			this.Enter("AddTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GetComponent<KPrefabID>().AddTag(tag);
			});
			this.Exit("RemoveTag(" + tag.Name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GetComponent<KPrefabID>().RemoveTag(tag);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(StatusItem status_item, Func<StateMachineInstanceType, object> callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				object obj = ((callback == null) ? null : callback(smi));
				Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, obj);
				smi.dataTable[data_idx] = guid;
			});
			this.Exit("RemoveStatusItem(" + status_item.Id + ")", delegate(StateMachineInstanceType smi)
			{
				KSelectable kselectable = state_target.Get<KSelectable>(smi);
				if (kselectable != null && smi.dataTable[data_idx] != null)
				{
					Guid guid2 = (Guid)smi.dataTable[data_idx];
					kselectable.RemoveStatusItem(guid2, false);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(Func<StateMachineInstanceType, StatusItem> status_item_cb, Func<StateMachineInstanceType, object> data_callback = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				StatusItem statusItem = status_item_cb(smi);
				object obj = ((data_callback == null) ? null : data_callback(smi));
				Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(statusItem, obj);
				smi.dataTable[data_idx] = guid;
			});
			this.Exit("RemoveStatusItem(DynamicallyConstructed)", delegate(StateMachineInstanceType smi)
			{
				KSelectable kselectable = state_target.Get<KSelectable>(smi);
				if (kselectable != null && smi.dataTable[data_idx] != null)
				{
					Guid guid2 = (Guid)smi.dataTable[data_idx];
					kselectable.RemoveStatusItem(guid2, false);
				}
				smi.dataTable[data_idx] = null;
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleFX(Func<StateMachineInstanceType, StateMachine.Instance> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("EnableFX()", delegate(StateMachineInstanceType smi)
			{
				StateMachine.Instance instance = callback(smi);
				if (instance != null)
				{
					instance.StartSM();
					smi.dataTable[data_idx] = instance;
				}
			});
			this.Exit("DisableFX()", delegate(StateMachineInstanceType smi)
			{
				object obj = smi.dataTable[data_idx];
				StateMachine.Instance instance2 = (StateMachine.Instance)obj;
				smi.dataTable[data_idx] = null;
				if (instance2 != null)
				{
					instance2.StopSM("ToggleFX.Exit");
				}
			});
			return this;
		}

		private void ClearChore(StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx)
		{
			Chore chore = (Chore)smi.dataTable[chore_data_idx];
			if (chore != null)
			{
				Action<Chore> action = (Action<Chore>)smi.dataTable[callback_data_idx];
				smi.dataTable[chore_data_idx] = null;
				smi.dataTable[callback_data_idx] = null;
				Chore chore2 = chore;
				chore2.onExit = (Action<Chore>)Delegate.Remove(chore2.onExit, action);
				chore.Cancel("ClearGlobalChore");
			}
		}

		private Chore SetupChore(Func<StateMachineInstanceType, Chore> create_chore_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state, StateMachineInstanceType smi, int chore_data_idx, int callback_data_idx, bool is_success_state_reentrant, bool is_failure_state_reentrant)
		{
			Chore chore = create_chore_callback(smi);
			chore.runUntilComplete = false;
			Action<Chore> action = delegate(Chore chore_param)
			{
				bool isComplete = chore.isComplete;
				bool flag = (isComplete && is_success_state_reentrant) || (is_failure_state_reentrant && !isComplete);
				if (flag)
				{
					this.SetupChore(create_chore_callback, success_state, failure_state, smi, chore_data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
				}
				else
				{
					GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state = success_state;
					if (!isComplete)
					{
						state = failure_state;
					}
					this.ClearChore(smi, chore_data_idx, callback_data_idx);
					smi.GoTo(state);
				}
			};
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, action);
			smi.dataTable[chore_data_idx] = chore;
			smi.dataTable[callback_data_idx] = action;
			return chore;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleRecurringChore(Func<StateMachineInstanceType, Chore> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			int callback_data_idx = this.CreateDataTableEntry();
			this.Enter("ToggleRecurringChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				this.SetupChore(callback, this, this, smi, data_idx, callback_data_idx, true, true);
			});
			this.Exit("ToggleRecurringChoreEnterExit()", delegate(StateMachineInstanceType smi)
			{
				this.ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleChore(Func<StateMachineInstanceType, Chore> callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
		{
			int data_idx = this.CreateDataTableEntry();
			int callback_data_idx = this.CreateDataTableEntry();
			this.Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				this.SetupChore(callback, target_state, target_state, smi, data_idx, callback_data_idx, false, false);
			});
			this.Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				this.ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleChore(Func<StateMachineInstanceType, Chore> callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
		{
			int data_idx = this.CreateDataTableEntry();
			int callback_data_idx = this.CreateDataTableEntry();
			bool is_success_state_reentrant = success_state == this;
			bool is_failure_state_reentrant = failure_state == this;
			this.Enter("ToggleChoreEnter()", delegate(StateMachineInstanceType smi)
			{
				this.SetupChore(callback, success_state, failure_state, smi, data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
			});
			this.Exit("ToggleChoreExit()", delegate(StateMachineInstanceType smi)
			{
				this.ClearChore(smi, data_idx, callback_data_idx);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleReactable(Func<StateMachineInstanceType, Reactable> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter(delegate(StateMachineInstanceType smi)
			{
				smi.dataTable[data_idx] = callback(smi);
			});
			this.Exit(delegate(StateMachineInstanceType smi)
			{
				Reactable reactable = (Reactable)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				if (reactable != null)
				{
					reactable.Cleanup();
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State RemoveEffect(string effect_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State SynchronizeAnims()
		{
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(string effect_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(effect_name, false);
			});
			this.Exit("RemoveEffect(" + effect_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(effect_name);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(Func<StateMachineInstanceType, Effect> callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), false);
			});
			this.Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(Func<StateMachineInstanceType, string> callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Add(callback(smi), false);
			});
			this.Exit("RemoveEffect()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Effects>(smi).Remove(callback(smi));
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Use<UsableType>(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter usable_target, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state) where UsableType : Usable
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter(string.Concat(new string[]
			{
				"StartUsing(",
				state_target.name,
				", ",
				typeof(UsableType).Name,
				")"
			}), delegate(StateMachineInstanceType smi)
			{
				User user = state_target.Get<User>(smi);
				GameObject gameObject = usable_target.Get(smi);
				if (gameObject == null)
				{
					smi.GoTo(failure_state);
				}
				else
				{
					UsableType component = gameObject.GetComponent<UsableType>();
					component.StartUsing(user);
				}
			});
			this.Exit(string.Concat(new string[]
			{
				"StopUsing(",
				state_target.name,
				", ",
				typeof(UsableType).Name,
				")"
			}), delegate(StateMachineInstanceType smi)
			{
				User user2 = state_target.Get<User>(smi);
				GameObject gameObject2 = usable_target.Get(smi);
				if (gameObject2 != null)
				{
					UsableType component2 = gameObject2.GetComponent<UsableType>();
					component2.StopUsing(user2);
				}
			});
			this.EventTransition(GameHashes.UseSuccess, success_state, null);
			this.EventTransition(GameHashes.UseFail, failure_state, null);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State LogOnExit(Func<StateMachineInstanceType, string> callback)
		{
			this.Enter("Log()", delegate(StateMachineInstanceType smi)
			{
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlaySoundOnEnter(string sound_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			string sound = GlobalAssets.GetSound(sound_name, false);
			this.Enter("PlaySound(" + sound_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KMonoBehaviour>(smi).PlaySound3D(sound);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlaySoundOnExit(string sound_name)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			string sound = GlobalAssets.GetSound(sound_name, false);
			this.Exit("PlaySound(" + sound_name + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<KMonoBehaviour>(smi).PlaySound3D(sound);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State LogOnEnter(Func<StateMachineInstanceType, string> callback)
		{
			this.Exit("Log()", delegate(StateMachineInstanceType smi)
			{
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleUrge(Urge urge)
		{
			return this.ToggleUrge((StateMachineInstanceType smi) => urge);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleUrge(Func<StateMachineInstanceType, Urge> urge_callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("AddUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge = urge_callback(smi);
				state_target.Get<ChoreConsumer>(smi).AddUrge(urge);
			});
			this.Exit("RemoveUrge()", delegate(StateMachineInstanceType smi)
			{
				Urge urge2 = urge_callback(smi);
				ChoreConsumer choreConsumer = state_target.Get<ChoreConsumer>(smi);
				if (choreConsumer != null)
				{
					choreConsumer.RemoveUrge(urge2);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnTargetLost(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter parameter, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
		{
			this.ParamTransition<GameObject>(parameter, target_state, (StateMachineInstanceType smi, GameObject p) => p == null);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleBrain(string reason)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("StopBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Stop(reason);
			});
			this.Exit("ResetBrain(" + reason + ")", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Brain>(smi).Reset(reason);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Stop()
		{
			this.Enter("Stop:", delegate(StateMachineInstanceType smi)
			{
				smi.StopSM("GameStateMachine.Stop");
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TriggerOnEnter(GameHashes evt, Func<StateMachineInstanceType, object> callback = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				object obj = ((callback == null) ? null : callback(smi));
				gameObject.Trigger((int)evt, obj);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TriggerOnExit(GameHashes evt)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Exit("Trigger(" + evt.ToString() + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = state_target.Get(smi);
				if (gameObject != null)
				{
					gameObject.Trigger((int)evt, null);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStateMachine(Func<StateMachineInstanceType, StateMachine.Instance> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("EnableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				StateMachine.Instance instance = callback(smi);
				smi.dataTable[data_idx] = instance;
				instance.StartSM();
			});
			this.Exit("DisableStateMachine()", delegate(StateMachineInstanceType smi)
			{
				StateMachine.Instance instance2 = (StateMachine.Instance)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				instance2.StopSM("ToggleStateMachine.Exit");
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStateMachine<ToggleType>() where ToggleType : StateMachineComponent
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableStateMachine(" + typeof(ToggleType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				ToggleType toggleType = state_target.Get<ToggleType>(smi);
				toggleType.GetSMI().StartSM();
			});
			this.Exit("DisableStateMachine(" + typeof(ToggleType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				ToggleType toggleType2 = state_target.Get<ToggleType>(smi);
				toggleType2.GetSMI().StopSM("ToggleStateMachine.Exit");
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleComponent<ComponentType>() where ComponentType : MonoBehaviour
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableComponent(" + typeof(ComponentType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				ComponentType componentType = state_target.Get<ComponentType>(smi);
				componentType.enabled = true;
			});
			this.Exit("DisableComponent(" + typeof(ComponentType).Name + ")", delegate(StateMachineInstanceType smi)
			{
				ComponentType componentType2 = state_target.Get<ComponentType>(smi);
				componentType2.enabled = false;
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleReserve(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter reserver, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_target, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter requested_amount, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter actual_amount)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter(string.Concat(new string[] { "Reserve(", pickup_target.name, ", ", requested_amount.name, ")" }), delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable = pickup_target.Get<Pickupable>(smi);
				GameObject gameObject = reserver.Get(smi);
				float num = requested_amount.Get(smi);
				float num2 = 600f;
				AttributeConverterInstance converter = gameObject.GetComponent<AttributeConverters>().GetConverter(Db.Get().AttributeConverters.CarryAmount.Id);
				num2 += converter.Evaluate();
				float num3 = Math.Min(num, num2);
				num3 = Math.Min(num3, pickupable.UnreservedAmount);
				if (num3 <= 0f)
				{
					pickupable.PrintReservations();
					global::Debug.LogError(string.Concat(new object[] { num2, ", ", num, ", ", pickupable.UnreservedAmount, ", ", num3 }), null);
				}
				actual_amount.Set(num3, smi);
				int num4 = pickupable.Reserve("ToggleReserve", gameObject, num3);
				smi.dataTable[data_idx] = num4;
			});
			this.Exit(string.Concat(new string[] { "Unreserve(", pickup_target.name, ", ", requested_amount.name, ")" }), delegate(StateMachineInstanceType smi)
			{
				int num5 = (int)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				Pickupable pickupable2 = pickup_target.Get<Pickupable>(smi);
				if (pickupable2 != null)
				{
					pickupable2.Unreserve("ToggleReserve", num5);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleWork(string work_type, Action<StateMachineInstanceType> callback, Func<StateMachineInstanceType, bool> validate_callback, Action<StateMachineInstanceType> complete_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("StartWork(" + work_type + ")", delegate(StateMachineInstanceType smi)
			{
				if (validate_callback(smi))
				{
					callback(smi);
				}
				else
				{
					smi.GoTo(failure_state);
				}
			});
			this.Update("Work()", delegate(StateMachineInstanceType smi)
			{
				if (validate_callback(smi))
				{
					Worker worker = state_target.Get<Worker>(smi);
					if (worker.Work())
					{
						worker.CompleteWork();
						if (complete_callback != null)
						{
							complete_callback(smi);
						}
						smi.GoTo(success_state);
					}
				}
				else
				{
					smi.GoTo(failure_state);
				}
			});
			this.Exit("StopWork()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Worker>(smi).StopWork();
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleWork<WorkableType>(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state) where WorkableType : Workable
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.ToggleWork(typeof(WorkableType).Name, delegate(StateMachineInstanceType smi)
			{
				Workable workable = source_target.Get<WorkableType>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StateMachineInstanceType smi) => source_target.Get<WorkableType>(smi) != null, null, success_state, failure_state);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoEat(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter amount, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.ToggleWork("Eat", delegate(StateMachineInstanceType smi)
			{
				Edible edible = source_target.Get<Edible>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float num = amount.Get(smi);
				worker.StartWork(new Edible.EdibleStartWorkInfo(edible, num));
			}, (StateMachineInstanceType smi) => source_target.Get<Edible>(smi) != null, null, success_state, failure_state);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoSleep(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter sleeper, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter bed, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.ToggleWork("Sleep", delegate(StateMachineInstanceType smi)
			{
				Worker worker = state_target.Get<Worker>(smi);
				Sleepable sleepable = bed.Get<Sleepable>(smi);
				worker.StartWork(new Worker.StartWorkInfo(sleepable));
			}, (StateMachineInstanceType smi) => bed.Get<Sleepable>(smi) != null, null, success_state, failure_state);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoPickup(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter result_target, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter amount, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.ToggleWork("Pickup", delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable = source_target.Get<Pickupable>(smi);
				Worker worker = state_target.Get<Worker>(smi);
				float num = amount.Get(smi);
				worker.StartWork(new Pickupable.PickupableStartWorkInfo(pickupable, num));
			}, (StateMachineInstanceType smi) => source_target.Get<Pickupable>(smi) != null || result_target.Get<Pickupable>(smi) != null, delegate(StateMachineInstanceType smi)
			{
				Pickupable pickupable2 = (Pickupable)state_target.Get<Worker>(smi).workCompleteData;
				if (pickupable2 == null || pickupable2.TotalAmount <= 0f)
				{
					result_target.Set(null, smi);
				}
				else
				{
					result_target.Set(pickupable2.gameObject, smi);
				}
			}, success_state, failure_state);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("EnableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = callback(smi);
				smi.dataTable[data_idx] = notification;
				state_target.Get<Notifier>(smi).Add(notification, string.Empty);
			});
			this.Exit("DisableNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification2 = (Notification)smi.dataTable[data_idx];
				if (notification2 != null)
				{
					if (state_target != null)
					{
						Notifier notifier = state_target.Get<Notifier>(smi);
						if (notifier != null)
						{
							notifier.Remove(notification2);
						}
					}
					smi.dataTable[data_idx] = null;
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoReport(ReportManager.ReportType reportType, Func<StateMachineInstanceType, float> callback)
		{
			this.Enter("DoReport()", delegate(StateMachineInstanceType smi)
			{
				float num = callback(smi);
				ReportManager.Instance.ReportValue(reportType, num, null);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoNotification(Func<StateMachineInstanceType, Notification> callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("DoNotification()", delegate(StateMachineInstanceType smi)
			{
				Notification notification = callback(smi);
				state_target.Get<Notifier>(smi).Add(notification, string.Empty);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoTutorial(Tutorial.TutorialMessages msg)
		{
			this.Enter("DoTutorial()", delegate(StateMachineInstanceType smi)
			{
				Tutorial.Instance.TutorialMessage(msg);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleScheduleUIPeriodic(string name, Func<StateMachineInstanceType, float> interval_callback, Action<StateMachineInstanceType> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddPeriodic(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				float num = interval_callback(smi);
				SchedulerHandle schedulerHandle = UIScheduler.Instance.SchedulePeriodic(name, num, delegate(object data)
				{
					callback(smi);
				}, smi, null);
				smi.dataTable[data_idx] = schedulerHandle;
			});
			this.Exit("RemovePeriodic(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				SchedulerHandle schedulerHandle2 = (SchedulerHandle)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				schedulerHandle2.ClearScheduler();
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleScheduleUIPeriodic(string name, float interval, Action<StateMachineInstanceType> callback)
		{
			return this.ToggleScheduleUIPeriodic(name, (StateMachineInstanceType smi) => interval, callback);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleSchedulePeriodic(string name, Func<StateMachineInstanceType, float> interval_callback, Action<StateMachineInstanceType> callback, Func<StateMachineInstanceType, GameObject> profiler_obj_cb = null)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddPeriodic(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				GameObject gameObject = null;
				float num = interval_callback(smi);
				GameScheduler instance = GameScheduler.Instance;
				GameObject gameObject2 = gameObject;
				SchedulerHandle schedulerHandle = instance.SchedulePeriodic(name, num, delegate(object data)
				{
					callback(smi);
				}, smi, null, 0f, gameObject2);
				DebugUtil.Assert(smi.dataTable[data_idx] == null, "Assert!");
				smi.dataTable[data_idx] = schedulerHandle;
			});
			this.Exit("RemovePeriodic(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				if (smi.dataTable[data_idx] == null)
				{
					return;
				}
				SchedulerHandle schedulerHandle2 = (SchedulerHandle)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				schedulerHandle2.ClearScheduler();
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleScheduleCallback(string name, float time, Action<StateMachineInstanceType> callback)
		{
			int data_idx = this.CreateDataTableEntry();
			this.Enter("AddScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				SchedulerHandle schedulerHandle = GameScheduler.Instance.Schedule(name, time, delegate(object smi_data)
				{
					callback((StateMachineInstanceType)((object)smi_data));
				}, smi, null);
				DebugUtil.Assert(smi.dataTable[data_idx] == null, "Assert!");
				smi.dataTable[data_idx] = schedulerHandle;
			});
			this.Exit("RemoveScheduledCallback(" + name + ")", delegate(StateMachineInstanceType smi)
			{
				SchedulerHandle schedulerHandle2 = (SchedulerHandle)smi.dataTable[data_idx];
				smi.dataTable[data_idx] = null;
				schedulerHandle2.ClearScheduler();
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleSchedulePeriodic(string name, float interval, Action<StateMachineInstanceType> callback)
		{
			return this.ToggleSchedulePeriodic(name, (StateMachineInstanceType smi) => interval, callback, null);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleGoTo(Func<StateMachineInstanceType, float> time_cb, StateMachine.BaseState state)
		{
			this.Enter("ScheduleGoTo(" + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time_cb(smi), state);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleGoTo(float time, StateMachine.BaseState state)
		{
			this.Enter(string.Concat(new string[]
			{
				"ScheduleGoTo(",
				time.ToString(),
				", ",
				state.name,
				")"
			}), delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(time, state);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleGoTo(Func<float> delay_cb, StateMachine.BaseState state)
		{
			this.Enter("ScheduleGoTo([callback_specified], " + state.name + ")", delegate(StateMachineInstanceType smi)
			{
				smi.ScheduleGoTo(delay_cb(), state);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			return this.EventHandler(evt, global_event_system_callback, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback)
		{
			List<StateEvent> list;
			if (this.events != null)
			{
				list = new List<StateEvent>(this.events);
			}
			else
			{
				list = new List<StateEvent>();
			}
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter targetParameter = this.GetStateTarget();
			GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent gameEvent = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent(evt, callback, targetParameter, global_event_system_callback);
			list.Add(gameEvent);
			this.events = list.ToArray();
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(GameHashes evt, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
		{
			return this.EventHandler(evt, delegate(StateMachineInstanceType smi, object d)
			{
				callback(smi);
			});
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(GameHashes evt, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback)
		{
			this.EventHandler(evt, null, callback);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ParamTransition<ParameterType>(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType> parameter, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Callback callback)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition[] array;
			if (this.parameterTransitions == null)
			{
				array = new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition[1];
			}
			else
			{
				array = new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ParameterTransition[this.parameterTransitions.Length + 1];
				Array.Copy(this.parameterTransitions, array, this.parameterTransitions.Length);
			}
			array[array.Length - 1] = new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Transition(parameter, state, callback);
			this.parameterTransitions = array;
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnSignal(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal signal, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, Func<StateMachineInstanceType, bool> callback)
		{
			this.ParamTransition<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>(signal, state, (StateMachineInstanceType smi, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter p) => callback(smi));
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnSignal(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal signal, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
		{
			this.ParamTransition<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>(signal, state, null);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Transition(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition)
		{
			string text = "(Stop)";
			if (state != null)
			{
				text = state.name;
			}
			this.Enter("Transition(" + text + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			this.Update("Transition(" + text + ")", delegate(StateMachineInstanceType smi)
			{
				if (condition(smi))
				{
					smi.GoTo(state);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DefaultState(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State default_state)
		{
			this.defaultState = default_state;
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State GoTo(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
		{
			string text = "(null)";
			if (state != null)
			{
				text = state.name;
			}
			this.Update("GoTo(" + text + ")", delegate(StateMachineInstanceType smi)
			{
				smi.GoTo(state);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State StopMoving()
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target = this.GetStateTarget();
			this.Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				target.Get<Navigator>(smi).Stop(false);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State MoveTo(Func<StateMachineInstanceType, int> cell_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state = null, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State fail_state = null, bool update_cell = false)
		{
			this.EventTransition(GameHashes.DestinationReached, success_state, null);
			this.EventTransition(GameHashes.NavigationFailed, fail_state, null);
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("MoveTo()", delegate(StateMachineInstanceType smi)
			{
				int num = cell_callback(smi);
				Navigator navigator = state_target.Get<Navigator>(smi);
				navigator.GoTo(num, null);
			});
			if (update_cell)
			{
				this.Update("MoveTo()", delegate(StateMachineInstanceType smi)
				{
					int num2 = cell_callback(smi);
					Navigator navigator2 = state_target.Get<Navigator>(smi);
					navigator2.UpdateTarget(num2);
				});
			}
			this.Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get(smi).GetComponent<Navigator>().Stop(false);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State MoveTo<ApproachableType>(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter move_parameter, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State fail_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null) where ApproachableType : IApproachable
		{
			this.EventTransition(GameHashes.DestinationReached, success_state, null);
			this.EventTransition(GameHashes.NavigationFailed, fail_state, null);
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			CellOffset[] offsets;
			this.Enter("MoveTo(" + move_parameter.name + ")", delegate(StateMachineInstanceType smi)
			{
				offsets = override_offsets;
				IApproachable approachable = move_parameter.Get<ApproachableType>(smi);
				KMonoBehaviour kmonoBehaviour = move_parameter.Get<KMonoBehaviour>(smi);
				if (kmonoBehaviour == null)
				{
					smi.GoTo(fail_state);
				}
				else
				{
					GameObject gameObject = state_target.Get(smi);
					Navigator component = gameObject.GetComponent<Navigator>();
					if (offsets == null)
					{
						offsets = approachable.GetOffsets();
					}
					component.GoTo(kmonoBehaviour, offsets, tactic);
				}
			});
			this.Exit("StopMoving()", delegate(StateMachineInstanceType smi)
			{
				state_target.Get<Navigator>(smi).Stop(false);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TagTransition(Tag[] tags, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, bool on_remove = false)
		{
			List<StateMachine.BaseTransition> list;
			if (this.transitions != null)
			{
				list = new List<StateMachine.BaseTransition>(this.transitions);
			}
			else
			{
				list = new List<StateMachine.BaseTransition>();
			}
			GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TagTransitionData tagTransitionData = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TagTransitionData(this, state, list.Count, tags, on_remove, this.GetStateTarget());
			list.Add(tagTransitionData);
			this.transitions = list.ToArray();
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TagTransition(Tag tag, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, bool on_remove = false)
		{
			return this.TagTransition(new Tag[] { tag }, state, on_remove);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventTransition(GameHashes evt, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition = null)
		{
			List<StateMachine.BaseTransition> list;
			if (this.transitions != null)
			{
				list = new List<StateMachine.BaseTransition>(this.transitions);
			}
			else
			{
				list = new List<StateMachine.BaseTransition>();
			}
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter targetParameter = this.GetStateTarget();
			GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.EventTransitionData eventTransitionData = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.EventTransitionData(this, state, list.Count, evt, global_event_system_callback, condition, targetParameter);
			list.Add(eventTransitionData);
			this.transitions = list.ToArray();
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventTransition(GameHashes evt, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition = null)
		{
			return this.EventTransition(evt, null, state, condition);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Return()
		{
			this.Enter("Return()", delegate(StateMachineInstanceType smi)
			{
				smi.StopSM("GameStateMachine.Return");
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ReturnSuccess()
		{
			this.Enter("ReturnSuccess()", delegate(StateMachineInstanceType smi)
			{
				smi.SetStatus(StateMachine.Status.Success);
				smi.StopSM("GameStateMachine.ReturnSuccess()");
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Success()
		{
			this.Enter("Success()", delegate(StateMachineInstanceType smi)
			{
				smi.SetStatus(StateMachine.Status.Success);
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnim(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once, Func<StateMachineInstanceType, string> suffix_callback = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter(string.Concat(new string[]
			{
				"PlayAnim(",
				anim,
				", ",
				mode.ToString(),
				")"
			}), delegate(StateMachineInstanceType smi)
			{
				string text = string.Empty;
				if (suffix_callback != null)
				{
					text = suffix_callback(smi);
				}
				KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kanimControllerBase != null)
				{
					kanimControllerBase.Play(anim + text, mode, 1f, 0f);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State QueueAnim(string anim, bool loop = false, Func<StateMachineInstanceType, string> suffix_callback = null)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			KAnim.PlayMode mode = KAnim.PlayMode.Once;
			if (loop)
			{
				mode = KAnim.PlayMode.Loop;
			}
			this.Enter(string.Concat(new string[]
			{
				"QueueAnim(",
				anim,
				", ",
				mode.ToString(),
				")"
			}), delegate(StateMachineInstanceType smi)
			{
				string text = string.Empty;
				if (suffix_callback != null)
				{
					text = suffix_callback(smi);
				}
				KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kanimControllerBase != null)
				{
					kanimControllerBase.Queue(anim + text, mode, 1f, 0f);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kanimControllerBase != null)
				{
					HashedString[] array = anims_callback(smi);
					kanimControllerBase.Play(array, mode);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnims(Func<StateMachineInstanceType, HashedString[]> anims_callback, Func<StateMachineInstanceType, KAnim.PlayMode> mode_cb)
		{
			StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
			this.Enter("PlayAnims", delegate(StateMachineInstanceType smi)
			{
				KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
				if (kanimControllerBase != null)
				{
					HashedString[] array = anims_callback(smi);
					KAnim.PlayMode playMode = mode_cb(smi);
					kanimControllerBase.Play(array, playMode);
				}
			});
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnAnimQueueComplete(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
		{
			return this.EventTransition(GameHashes.AnimQueueComplete, state, null);
		}

		[StateMachine.DoNotAutoCreate]
		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter stateTarget;

		private static object HasToggleEnteredFlag = new object();
	}

	public class GameEvent : StateEvent
	{
		public GameEvent(GameHashes id, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target, Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback)
			: base(id.ToString())
		{
			this.id = id;
			this.target = target;
			this.callback = callback;
			this.globalEventSystemCallback = global_event_system_callback;
		}

		public override StateEvent.Context Subscribe(StateMachine.Instance smi)
		{
			StateEvent.Context context = base.Subscribe(smi);
			StateMachineInstanceType cast_smi = (StateMachineInstanceType)((object)smi);
			Action<object> action = delegate(object d)
			{
				if (StateMachine.Instance.error)
				{
					return;
				}
				this.callback(cast_smi, d);
			};
			if (this.globalEventSystemCallback != null)
			{
				KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(cast_smi);
				context.data = kmonoBehaviour.Subscribe((int)this.id, action);
			}
			else
			{
				context.data = this.target.Get(cast_smi).Subscribe((int)this.id, action);
			}
			return context;
		}

		public override void Unsubscribe(StateMachine.Instance smi, StateEvent.Context context)
		{
			StateMachineInstanceType stateMachineInstanceType = (StateMachineInstanceType)((object)smi);
			if (this.globalEventSystemCallback != null)
			{
				KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(stateMachineInstanceType);
				if (kmonoBehaviour != null)
				{
					kmonoBehaviour.Unsubscribe(context.data);
				}
			}
			else
			{
				GameObject gameObject = this.target.Get(stateMachineInstanceType);
				if (gameObject != null)
				{
					gameObject.Unsubscribe(context.data);
				}
			}
		}

		private GameHashes id;

		private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;

		private GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback;

		private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

		public delegate void Callback(StateMachineInstanceType smi, object callback_data);
	}

	public class ApproachSubState<ApproachableType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State where ApproachableType : IApproachable
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter mover, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter move_target, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null, CellOffset[] override_offsets = null, NavTactic tactic = null)
		{
			base.root.Target(mover).OnTargetLost(move_target, failure_state).MoveTo<ApproachableType>(move_target, success_state, failure_state, override_offsets, (tactic != null) ? tactic : NavigationTactics.ReduceTravelDistance);
			return this;
		}
	}

	public class CreatureFleeSubState<ApproachableType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State where ApproachableType : IApproachable
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter mover, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state)
		{
			this.defaultState = this.plan;
			base.root.Target(mover).ToggleStatusItem(Db.Get().CreatureStatusItems.Fleeing, null);
			this.plan.Enter(delegate(StateMachineInstanceType smi)
			{
				MasterType master = smi.master;
				ThreatMonitor.Instance smi2 = master.gameObject.GetSMI<ThreatMonitor.Instance>();
				StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject> parameter = this.fleeToTarget;
				MasterType master2 = smi.master;
				parameter.Set(CreatureHelpers.GetFleeTargetLocatorObject(master2.gameObject, smi2.GetMainThreat), smi);
				if (this.fleeToTarget.Get(smi) != null)
				{
					smi.GoTo(this.approach);
				}
				else
				{
					smi.GoTo(this.cower);
				}
			});
			this.approach.InitializeStates(mover, this.fleeToTarget, this.cower, this.cower, null, NavigationTactics.ReduceTravelDistance).Enter(delegate(StateMachineInstanceType smi)
			{
				PopFXManager instance = PopFXManager.Instance;
				Sprite sprite_Plus = PopFXManager.Instance.sprite_Plus;
				string text = CREATURES.STATUSITEMS.FLEEING.NAME.text;
				MasterType master3 = smi.master;
				instance.SpawnFX(sprite_Plus, text, master3.transform, 1.5f, false);
			});
			this.cower.Enter(delegate(StateMachineInstanceType smi)
			{
				string text2 = "DEFAULT COWER ANIMATION";
				if (smi.animController.HasAnimation("cower"))
				{
					text2 = "cower";
				}
				else if (smi.animController.HasAnimation("idle"))
				{
					text2 = "idle";
				}
				else if (smi.animController.HasAnimation("idle_loop"))
				{
					text2 = "idle_loop";
				}
				smi.animController.Play(text2, KAnim.PlayMode.Loop, 1f, 0f);
			}).ScheduleGoTo(2f, success_state);
			return this;
		}

		public StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter fleeToTarget;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State plan;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ApproachSubState<Approachable> approach;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State cower;
	}

	public class DebugGoToSubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State exit_state)
		{
			base.root.Enter("GoToCursor", delegate(StateMachineInstanceType smi)
			{
				this.GoToCursor(smi);
			}).EventHandler(GameHashes.DebugGoTo, (StateMachineInstanceType smi) => Game.Instance, delegate(StateMachineInstanceType smi)
			{
				this.GoToCursor(smi);
			}).EventTransition(GameHashes.DestinationReached, exit_state, null)
				.EventTransition(GameHashes.NavigationFailed, exit_state, null);
			return this;
		}

		public void GoToCursor(StateMachineInstanceType smi)
		{
			smi.GetComponent<Navigator>().GoTo(Grid.PosToCell(DebugHandler.GetMousePos()), Grid.DefaultOffset);
		}
	}

	public class DropSubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter carrier, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter item, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter drop_target, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null)
		{
			base.root.Target(carrier).Enter("Drop", delegate(StateMachineInstanceType smi)
			{
				Storage storage = carrier.Get<Storage>(smi);
				GameObject gameObject = item.Get(smi);
				storage.Drop(gameObject);
				Transform transform = drop_target.Get<Transform>(smi);
				int num = Grid.PosToCell(transform.position);
				int num2 = Grid.CellAbove(num);
				gameObject.transform.SetPosition(Grid.CellToPosCCC(num2, Grid.SceneLayer.Move));
				smi.GoTo(success_state);
			});
			return this;
		}
	}

	public class FetchSubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter fetcher, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_source, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_chunk, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter requested_amount, StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter actual_amount, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null)
		{
			base.Target(fetcher);
			base.root.DefaultState(this.approach).ToggleReserve(fetcher, pickup_source, requested_amount, actual_amount);
			GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ApproachSubState<Pickupable> approachSubState = this.approach;
			NavTactic reduceTravelDistance = NavigationTactics.ReduceTravelDistance;
			approachSubState.InitializeStates(fetcher, pickup_source, this.pickup, null, null, reduceTravelDistance).OnTargetLost(pickup_source, failure_state);
			this.pickup.DoPickup(pickup_source, pickup_chunk, actual_amount, success_state, failure_state);
			return this;
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ApproachSubState<Pickupable> approach;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pickup;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success;
	}

	public class HungrySubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target, StatusItem status_item)
		{
			base.Target(target);
			base.root.DefaultState(this.satisfied);
			this.satisfied.EventTransition(GameHashes.AddUrge, this.hungry, (StateMachineInstanceType smi) => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.HungrySubState.IsHungry(smi));
			this.hungry.EventTransition(GameHashes.RemoveUrge, this.satisfied, (StateMachineInstanceType smi) => !GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.HungrySubState.IsHungry(smi)).ToggleStatusItem(status_item, null);
			return this;
		}

		private static bool IsHungry(StateMachineInstanceType smi)
		{
			return smi.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
		}

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State satisfied;

		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State hungry;
	}

	public class IdleMoveSubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State exit_state)
		{
			base.root.Enter("GoToCursor", delegate(StateMachineInstanceType smi)
			{
				this.GoToCursor(smi);
			}).EventHandler(GameHashes.DebugGoTo, (StateMachineInstanceType smi) => Game.Instance, delegate(StateMachineInstanceType smi)
			{
				this.GoToCursor(smi);
			}).EventTransition(GameHashes.DestinationReached, exit_state, null)
				.EventTransition(GameHashes.NavigationFailed, exit_state, null);
			return this;
		}

		public void GoToCursor(StateMachineInstanceType smi)
		{
			GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.IdleMoveSubState.MoveCellQuery moveCellQuery = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.IdleMoveSubState.MoveCellQuery(smi.GetComponent<Navigator>().CurrentNavType);
			smi.GetComponent<Navigator>().RunQuery(moveCellQuery);
			smi.GetComponent<Navigator>().GoTo(moveCellQuery.GetResultCell(), Grid.DefaultOffset);
		}

		public class MoveCellQuery : PathFinderQuery
		{
			public MoveCellQuery(NavType navType)
			{
				this.navType = navType;
				this.maxIterations = global::UnityEngine.Random.Range(5, 25);
			}

			public override bool IsMatch(int cell, int parent_cell, int cost)
			{
				if (!Grid.IsValidCell(cell))
				{
					return false;
				}
				if (Grid.IsSubstantialLiquid(cell, 0.35f) == (this.navType == NavType.Swim))
				{
					this.targetCell = cell;
					return --this.maxIterations <= 0;
				}
				return false;
			}

			public override int GetResultCell()
			{
				return this.targetCell;
			}

			private NavType navType;

			private int targetCell = Grid.InvalidCell;

			private int maxIterations;
		}
	}

	public class PlantAliveSubState : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
	{
		public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter plant, GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State death_state = null)
		{
			base.root.Target(plant).EventTransition(GameHashes.Uprooted, death_state, (StateMachineInstanceType smi) => UprootedMonitor.IsObjectUprooted(plant.Get(smi))).EventTransition(GameHashes.TooColdFatal, death_state, (StateMachineInstanceType smi) => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.PlantAliveSubState.isLethalTemperature(plant.Get(smi)))
				.EventTransition(GameHashes.TooHotFatal, death_state, (StateMachineInstanceType smi) => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.PlantAliveSubState.isLethalTemperature(plant.Get(smi)))
				.TagTransition(GameTags.Entombed, death_state, false)
				.EventTransition(GameHashes.Drowned, death_state, null);
			return this;
		}

		public bool ForceUpdateStatus(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			EntombVulnerable component2 = plant.GetComponent<EntombVulnerable>();
			UprootedMonitor component3 = plant.GetComponent<UprootedMonitor>();
			PressureVulnerable component4 = plant.GetComponent<PressureVulnerable>();
			return (component == null || !component.IsLethal) && (component2 == null || !component2.GetEntombed) && (component3 == null || !component3.IsUprooted) && (component4 == null || !component4.IsLethal);
		}

		private static bool isLethalTemperature(GameObject plant)
		{
			TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
			return !(component == null) && (component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalCold || component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalHot);
		}
	}
}
