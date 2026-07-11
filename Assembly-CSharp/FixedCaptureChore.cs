using System;
using STRINGS;

public class FixedCaptureChore : Chore<FixedCaptureChore.FixedCaptureChoreStates.Instance>
{
	public FixedCaptureChore(KPrefabID capture_point)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsCreatureAvailableForFixedCapture";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_FIXED_CAPTURE;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			FixedCapturePoint.Instance instance = data as FixedCapturePoint.Instance;
			return instance.IsCreatureAvailableForFixedCapture();
		};
		this.IsCreatureAvailableForFixedCapture = precondition;
		base..ctor(Db.Get().ChoreTypes.Ranch, capture_point, null, false, null, null, null, PriorityScreen.PriorityClass.basic, 0, false, true, 0, null);
		base.AddPrecondition(this.IsCreatureAvailableForFixedCapture, capture_point.GetSMI<FixedCapturePoint.Instance>());
		base.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanWrangleCreatures.id);
		this.smi = new FixedCaptureChore.FixedCaptureChoreStates.Instance(capture_point);
		base.SetPrioritizable(capture_point.GetComponent<Prioritizable>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.rancher.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public Chore.Precondition IsCreatureAvailableForFixedCapture;

	public class FixedCaptureChoreStates : GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.movetopoint;
			base.Target(this.rancher);
			this.root.Exit("TriggerFixedCapturePointNoLongerAvailable", delegate(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
			{
				smi.TriggerFixedCapturePointNoLongerAvailable();
			});
			this.movetopoint.MoveTo((FixedCaptureChore.FixedCaptureChoreStates.Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), this.waitforcreature_pre, null, false).Target(this.masterTarget).EventTransition(GameHashes.CreatureAbandonedCapturePoint, this.checkformorecapturables, null);
			this.waitforcreature_pre.EnterTransition(null, (FixedCaptureChore.FixedCaptureChoreStates.Instance smi) => smi.fixedCapturePoint.IsNullOrStopped()).EnterTransition(this.checkformorecapturables, new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(FixedCaptureChore.FixedCaptureChoreStates.HasCreatureLeft)).EnterTransition(this.waitforcreature, (FixedCaptureChore.FixedCaptureChoreStates.Instance smi) => true);
			this.waitforcreature.ToggleAnims("anim_interacts_rancherstation_kanim", 0f).PlayAnim("calling_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.FaceCreature))
				.Enter("SetRancherIsAvailableForCapturing", delegate(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
				{
					smi.fixedCapturePoint.SetRancherIsAvailableForCapturing();
				})
				.Exit("ClearRancherIsAvailableForCapturing", delegate(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
				{
					smi.fixedCapturePoint.ClearRancherIsAvailableForCapturing();
				})
				.Transition(this.checkformorecapturables, new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(FixedCaptureChore.FixedCaptureChoreStates.HasCreatureLeft), UpdateRate.SIM_200ms)
				.Target(this.masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtCapturePoint, this.capturecreature, null);
			this.capturecreature.ToggleAnims("anim_interacts_rancherstation_kanim", 0f).DefaultState(this.capturecreature.pre).EventTransition(GameHashes.CreatureAbandonedCapturePoint, this.checkformorecapturables, null)
				.EnterTransition(this.checkformorecapturables, (FixedCaptureChore.FixedCaptureChoreStates.Instance smi) => smi.fixedCapturePoint.targetCapturable.IsNullOrStopped());
			this.capturecreature.pre.Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.FaceCreature)).Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.PlayBuildingWorkingPre)).QueueAnim("working_pre", false, null)
				.OnAnimQueueComplete(this.capturecreature.loop);
			this.capturecreature.loop.EnterTransition(this.checkformorecapturables, new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(FixedCaptureChore.FixedCaptureChoreStates.HasCreatureLeft)).Enter("TellCreatureRancherIsReady", delegate(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
			{
				smi.fixedCapturePoint.targetCapturable.Trigger(449143823, null);
			}).Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.PlayBuildingWorkingLoop))
				.Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.PlayRancherWorkingLoops))
				.Target(this.rancher)
				.OnAnimQueueComplete(this.capturecreature.pst);
			this.capturecreature.pst.Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.PlayBuildingWorkingPst)).QueueAnim("working_pst", false, null).QueueAnim("wipe_brow", false, null)
				.OnAnimQueueComplete(this.docapturecreature);
			this.docapturecreature.Enter(new StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State.Callback(FixedCaptureChore.FixedCaptureChoreStates.CaptureCreature)).GoTo(this.checkformorecapturables);
			this.checkformorecapturables.Enter("FindCaptureable", delegate(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
			{
				smi.CheckForMoreCapturables();
			});
		}

		private static bool HasCreatureLeft(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			return smi.fixedCapturePoint.targetCapturable.IsNullOrStopped() || !smi.fixedCapturePoint.targetCapturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>();
		}

		private static void FaceCreature(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			smi.sm.rancher.Get<Facing>(smi).Face(smi.fixedCapturePoint.targetCapturable.transform.GetPosition());
		}

		private static void CaptureCreature(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			smi.fixedCapturePoint.CaptureCreature();
		}

		private static bool ShouldSynchronizeBuilding(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			return smi.fixedCapturePoint.def.synchronizeBuilding;
		}

		private static void PlayBuildingWorkingPre(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			if (FixedCaptureChore.FixedCaptureChoreStates.ShouldSynchronizeBuilding(smi))
			{
				smi.fixedCapturePoint.GetComponent<KBatchedAnimController>().Queue("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private static void PlayRancherWorkingLoops(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			KBatchedAnimController kbatchedAnimController = smi.sm.rancher.Get<KBatchedAnimController>(smi);
			for (int i = 0; i < smi.fixedCapturePoint.def.interactLoopCount; i++)
			{
				kbatchedAnimController.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private static void PlayBuildingWorkingLoop(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			if (FixedCaptureChore.FixedCaptureChoreStates.ShouldSynchronizeBuilding(smi))
			{
				smi.fixedCapturePoint.GetComponent<KBatchedAnimController>().Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		private static void PlayBuildingWorkingPst(FixedCaptureChore.FixedCaptureChoreStates.Instance smi)
		{
			if (FixedCaptureChore.FixedCaptureChoreStates.ShouldSynchronizeBuilding(smi))
			{
				smi.fixedCapturePoint.GetComponent<KBatchedAnimController>().Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public StateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.TargetParameter rancher;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State movetopoint;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State waitforcreature_pre;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State waitforcreature;

		private FixedCaptureChore.FixedCaptureChoreStates.CaptureState capturecreature;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State wavegoodbye;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State docapturecreature;

		private GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State checkformorecapturables;

		private class CaptureState : GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State pre;

			public GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State loop;

			public GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.State pst;
		}

		public new class Instance : GameStateMachine<FixedCaptureChore.FixedCaptureChoreStates, FixedCaptureChore.FixedCaptureChoreStates.Instance, IStateMachineTarget, object>.GameInstance
		{
			public Instance(KPrefabID capture_point)
				: base(capture_point)
			{
				this.fixedCapturePoint = capture_point.GetSMI<FixedCapturePoint.Instance>();
			}

			public void CheckForMoreCapturables()
			{
				this.fixedCapturePoint.FindFixedCapturable();
				if (this.fixedCapturePoint.IsCreatureAvailableForFixedCapture())
				{
					this.GoTo(base.sm.movetopoint);
				}
				else
				{
					this.GoTo(null);
				}
			}

			public void TriggerFixedCapturePointNoLongerAvailable()
			{
				this.fixedCapturePoint.TriggerFixedCapturePointNoLongerAvailable();
			}

			public FixedCapturePoint.Instance fixedCapturePoint;
		}
	}
}
