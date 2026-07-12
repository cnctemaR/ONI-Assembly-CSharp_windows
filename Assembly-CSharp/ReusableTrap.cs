using System;
using System.Collections.Generic;
using UnityEngine;

public class ReusableTrap : GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.operational;
		this.noOperational.TagTransition(GameTags.Operational, this.operational, false).Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.RefreshLogicOutput)).DefaultState(this.noOperational.idle);
		this.noOperational.idle.EnterTransition(this.noOperational.releasing, new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.Transition.ConditionCallback(ReusableTrap.StorageContainsCritter)).ParamTransition<bool>(this.IsArmed, this.noOperational.disarming, GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.IsTrue).PlayAnim("off");
		this.noOperational.releasing.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.MarkAsUnarmed)).Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.Release)).PlayAnim("release")
			.OnAnimQueueComplete(this.noOperational.idle);
		this.noOperational.disarming.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.MarkAsUnarmed)).PlayAnim("abort_armed").OnAnimQueueComplete(this.noOperational.idle);
		this.operational.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.RefreshLogicOutput)).TagTransition(GameTags.Operational, this.noOperational, true).DefaultState(this.operational.unarmed);
		this.operational.unarmed.ParamTransition<bool>(this.IsArmed, this.operational.armed, GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.IsTrue).EnterTransition(this.operational.capture.idle, new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.Transition.ConditionCallback(ReusableTrap.StorageContainsCritter)).ToggleStatusItem(Db.Get().BuildingStatusItems.TrapNeedsArming, null)
			.PlayAnim("unarmed")
			.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.DisableTrapTrigger))
			.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.StartArmTrapWorkChore))
			.Exit(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.CancelArmTrapWorkChore))
			.WorkableCompleteTransition(new Func<ReusableTrap.Instance, Workable>(ReusableTrap.GetWorkable), this.operational.armed);
		this.operational.armed.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.MarkAsArmed)).EnterTransition(this.operational.capture.idle, new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.Transition.ConditionCallback(ReusableTrap.StorageContainsCritter)).PlayAnim("armed", KAnim.PlayMode.Loop)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.TrapArmed, null)
			.Toggle("Enable/Disable Trap Trigger", new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.EnableTrapTrigger), new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.DisableTrapTrigger))
			.Toggle("Enable/Disable Lure", new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.ActivateLure), new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.DisableLure))
			.EventHandlerTransition(GameHashes.TrapTriggered, this.operational.capture.capturing, new Func<ReusableTrap.Instance, object, bool>(ReusableTrap.HasCritter_OnTrapTriggered));
		this.operational.capture.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.RefreshLogicOutput)).Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.DisableTrapTrigger)).Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.MarkAsUnarmed))
			.ToggleTag(GameTags.Trapped)
			.DefaultState(this.operational.capture.capturing)
			.EventHandlerTransition(GameHashes.OnStorageChange, this.operational.capture.release, new Func<ReusableTrap.Instance, object, bool>(ReusableTrap.OnStorageEmptied));
		this.operational.capture.capturing.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.SetupCapturingAnimations)).Update(new Action<ReusableTrap.Instance, float>(ReusableTrap.OptionalCapturingAnimationUpdate), UpdateRate.RENDER_EVERY_TICK, false).PlayAnim("capture")
			.OnAnimQueueComplete(this.operational.capture.idle)
			.Exit(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.UnsetCapturingAnimations));
		this.operational.capture.idle.TriggerOnEnter(GameHashes.TrapCaptureCompleted, null).ToggleStatusItem(Db.Get().BuildingStatusItems.TrapHasCritter, (ReusableTrap.Instance smi) => smi.CapturedCritter).PlayAnim("capture_idle");
		this.operational.capture.release.Enter(new StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State.Callback(ReusableTrap.RefreshLogicOutput)).QueueAnim("release", false, null).OnAnimQueueComplete(this.operational.unarmed);
	}

	public static void RefreshLogicOutput(ReusableTrap.Instance smi)
	{
		smi.RefreshLogicOutput();
	}

	public static void Release(ReusableTrap.Instance smi)
	{
		smi.Release();
	}

	public static void StartArmTrapWorkChore(ReusableTrap.Instance smi)
	{
		smi.CreateWorkableChore();
	}

	public static void CancelArmTrapWorkChore(ReusableTrap.Instance smi)
	{
		smi.CancelWorkChore();
	}

	public static bool OnStorageEmptied(ReusableTrap.Instance smi, object obj)
	{
		return !smi.HasCritter;
	}

	public static bool HasCritter_OnTrapTriggered(ReusableTrap.Instance smi, object capturedItem)
	{
		return smi.HasCritter;
	}

	public static bool StorageContainsCritter(ReusableTrap.Instance smi)
	{
		return smi.HasCritter;
	}

	public static bool StorageIsEmpty(ReusableTrap.Instance smi)
	{
		return !smi.HasCritter;
	}

	public static void EnableTrapTrigger(ReusableTrap.Instance smi)
	{
		smi.SetTrapTriggerActiveState(true);
	}

	public static void DisableTrapTrigger(ReusableTrap.Instance smi)
	{
		smi.SetTrapTriggerActiveState(false);
	}

	public static ArmTrapWorkable GetWorkable(ReusableTrap.Instance smi)
	{
		return smi.GetWorkable();
	}

	public static void ActivateLure(ReusableTrap.Instance smi)
	{
		smi.SetLureActiveState(true);
	}

	public static void DisableLure(ReusableTrap.Instance smi)
	{
		smi.SetLureActiveState(false);
	}

	public static void SetupCapturingAnimations(ReusableTrap.Instance smi)
	{
		smi.SetupCapturingAnimations();
	}

	public static void UnsetCapturingAnimations(ReusableTrap.Instance smi)
	{
		smi.UnsetCapturingAnimations();
	}

	public static void OptionalCapturingAnimationUpdate(ReusableTrap.Instance smi, float dt)
	{
		if (smi.def.usingSymbolChaseCapturingAnimations && smi.lastCritterCapturedAnimController != null)
		{
			if (smi.lastCritterCapturedAnimController.currentAnim != smi.CAPTURING_CRITTER_ANIMATION_NAME)
			{
				smi.lastCritterCapturedAnimController.Play(smi.CAPTURING_CRITTER_ANIMATION_NAME, KAnim.PlayMode.Once, 1f, 0f);
			}
			bool flag;
			Vector3 vector = smi.animController.GetSymbolTransform(smi.CAPTURING_SYMBOL_NAME, out flag).GetColumn(3);
			smi.lastCritterCapturedAnimController.transform.SetPosition(vector);
		}
	}

	public static void MarkAsArmed(ReusableTrap.Instance smi)
	{
		smi.sm.IsArmed.Set(true, smi, false);
		smi.gameObject.AddTag(GameTags.TrapArmed);
	}

	public static void MarkAsUnarmed(ReusableTrap.Instance smi)
	{
		smi.sm.IsArmed.Set(false, smi, false);
		smi.gameObject.RemoveTag(GameTags.TrapArmed);
	}

	public const string CAPTURE_ANIMATION_NAME = "capture";

	public const string CAPTURE_IDLE_ANIMATION_NAME = "capture_idle";

	public const string CAPTURE_RELEASE_ANIMATION_NAME = "release";

	public const string UNARMED_ANIMATION_NAME = "unarmed";

	public const string ARMED_ANIMATION_NAME = "armed";

	public const string ABORT_ARMED_ANIMATION = "abort_armed";

	public StateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.BoolParameter IsArmed;

	public ReusableTrap.NonOperationalStates noOperational;

	public ReusableTrap.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
		public bool usingLure
		{
			get
			{
				return this.lures != null && this.lures.Length != 0;
			}
		}

		public string OUTPUT_LOGIC_PORT_ID;

		public Tag[] lures;

		public CellOffset releaseCellOffset = CellOffset.none;

		public bool usingSymbolChaseCapturingAnimations;

		public Func<string> getTrappedAnimationNameCallback;
	}

	public class CaptureStates : GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State
	{
		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State capturing;

		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State idle;

		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State release;
	}

	public class OperationalStates : GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State
	{
		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State unarmed;

		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State armed;

		public ReusableTrap.CaptureStates capture;
	}

	public class NonOperationalStates : GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State
	{
		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State idle;

		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State releasing;

		public GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.State disarming;
	}

	public new class Instance : GameStateMachine<ReusableTrap, ReusableTrap.Instance, IStateMachineTarget, ReusableTrap.Def>.GameInstance, TrappedStates.ITrapStateAnimationInstructions
	{
		public bool HasCritter
		{
			get
			{
				return !this.storage.IsEmpty();
			}
		}

		public GameObject CapturedCritter
		{
			get
			{
				if (!this.HasCritter)
				{
					return null;
				}
				return this.storage.items[0];
			}
		}

		public ArmTrapWorkable GetWorkable()
		{
			return this.workable;
		}

		public void RefreshLogicOutput()
		{
			bool flag = base.IsInsideState(base.sm.operational) && this.HasCritter;
			this.logicPorts.SendSignal(base.def.OUTPUT_LOGIC_PORT_ID, flag ? 1 : 0);
		}

		public Instance(IStateMachineTarget master, ReusableTrap.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			ArmTrapWorkable armTrapWorkable = this.workable;
			armTrapWorkable.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(armTrapWorkable.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnWorkEvent));
		}

		private void OnWorkEvent(Workable workable, Workable.WorkableEvent state)
		{
			if (state == Workable.WorkableEvent.WorkStopped && workable.GetPercentComplete() < 1f && workable.GetPercentComplete() != 0f && base.IsInsideState(base.sm.operational.unarmed))
			{
				this.animController.Play("unarmed", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public void SetTrapTriggerActiveState(bool active)
		{
			this.trapTrigger.enabled = active;
		}

		public void SetLureActiveState(bool activate)
		{
			if (base.def.usingLure)
			{
				Lure.Instance smi = base.gameObject.GetSMI<Lure.Instance>();
				if (smi != null)
				{
					smi.SetActiveLures(activate ? base.def.lures : null);
				}
			}
		}

		public void SetupCapturingAnimations()
		{
			if (this.HasCritter)
			{
				this.lastCritterCapturedAnimController = this.CapturedCritter.GetComponent<KBatchedAnimController>();
			}
		}

		public void UnsetCapturingAnimations()
		{
			this.trapTrigger.SetStoredPosition(this.CapturedCritter);
			if (base.def.usingSymbolChaseCapturingAnimations && this.lastCritterCapturedAnimController != null)
			{
				this.lastCritterCapturedAnimController.Play("trapped", KAnim.PlayMode.Loop, 1f, 0f);
			}
			this.lastCritterCapturedAnimController = null;
		}

		public void CreateWorkableChore()
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<ArmTrapWorkable>(Db.Get().ChoreTypes.ArmTrap, this.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelWorkChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("GroundTrap.CancelChore");
				this.chore = null;
			}
		}

		public void Release()
		{
			if (this.HasCritter)
			{
				Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(base.smi.transform.GetPosition()), base.def.releaseCellOffset), Grid.SceneLayer.Creatures);
				List<GameObject> list = new List<GameObject>();
				this.storage.DropAll(false, false, default(Vector3), true, list);
				foreach (GameObject gameObject in list)
				{
					gameObject.transform.SetPosition(vector);
					KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.SetSceneLayer(Grid.SceneLayer.Creatures);
					}
				}
			}
		}

		public string GetTrappedAnimationName()
		{
			if (base.def.getTrappedAnimationNameCallback != null)
			{
				return base.def.getTrappedAnimationNameCallback();
			}
			return null;
		}

		public string CAPTURING_CRITTER_ANIMATION_NAME = "caught_loop";

		public string CAPTURING_SYMBOL_NAME = "creatureSymbol";

		[MyCmpGet]
		private Storage storage;

		[MyCmpGet]
		private ArmTrapWorkable workable;

		[MyCmpGet]
		private TrapTrigger trapTrigger;

		[MyCmpGet]
		public KBatchedAnimController animController;

		[MyCmpGet]
		public LogicPorts logicPorts;

		public KBatchedAnimController lastCritterCapturedAnimController;

		private Chore chore;
	}
}
