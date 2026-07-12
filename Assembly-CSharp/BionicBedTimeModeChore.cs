using System;
using UnityEngine;

public class BionicBedTimeModeChore : Chore<BionicBedTimeModeChore.Instance>
{
	public BionicBedTimeModeChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BionicBedtimeMode, master, master.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BionicBedTimeModeChore.Instance(this, master.gameObject);
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
	}

	public static void BeginWorkOnZone(BionicBedTimeModeChore.Instance smi)
	{
		WorkerBase workerBase = smi.sm.bionic.Get<WorkerBase>(smi);
		DefragmentationZone assignedDefragmentationZone = smi.GetAssignedDefragmentationZone();
		workerBase.StartWork(new WorkerBase.StartWorkInfo(assignedDefragmentationZone));
	}

	public static bool HasDefragmentationZoneAssignedAndReachable(BionicBedTimeModeChore.Instance smi, GameObject defragmentationZone)
	{
		return defragmentationZone != null && smi.IsDefragmentationZoneReachable();
	}

	public static bool HasDefragmentationZoneAssignedAndReachable(BionicBedTimeModeChore.Instance smi)
	{
		return smi.sm.defragmentationZone.Get(smi) != null && smi.IsDefragmentationZoneReachable();
	}

	public static bool IsBedTimeAllowed(BionicBedTimeModeChore.Instance smi)
	{
		return BionicBedTimeMonitor.CanGoToBedTime(smi.bedTimeMonitor);
	}

	public static void UpdateAssignedDefragmentationZone(BionicBedTimeModeChore.Instance smi)
	{
		smi.UpdateAssignedDefragmentationZone(null);
	}

	public const string EFFECT_NAME = "BionicBedTimeEffect";

	public class States : GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.enter;
			this.root.ToggleEffect("BionicBedTimeEffect");
			this.enter.Transition(null, GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Not(new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Transition.ConditionCallback(BionicBedTimeModeChore.IsBedTimeAllowed)), UpdateRate.SIM_200ms).ParamTransition<GameObject>(this.defragmentationZone, this.approach, new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Parameter<GameObject>.Callback(BionicBedTimeModeChore.HasDefragmentationZoneAssignedAndReachable)).GoTo(this.defragmentingWithoutAssignable);
			this.unassigning.ScheduleActionNextFrame("Frame delay on unassign", delegate(BionicBedTimeModeChore.Instance smi)
			{
				BionicBedTimeModeChore.UpdateAssignedDefragmentationZone(smi);
				smi.GoTo(this.enter);
			});
			this.approach.InitializeStates(this.bionic, this.defragmentationZone, this.defragmentingOnAssignable, null, null, null).OnSignal(this.defragmentationZoneUnassignined, this.unassigning).ScheduleChange(null, GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Not(new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Transition.ConditionCallback(BionicBedTimeModeChore.IsBedTimeAllowed)))
				.EventTransition(GameHashes.BionicOffline, null, null);
			this.defragmentingOnAssignable.OnTargetLost(this.defragmentationZone, this.defragmentingWithoutAssignable).OnSignal(this.defragmentationZoneChangedSignal, this.enter).OnSignal(this.defragmentationZoneUnassignined, this.unassigning)
				.EventTransition(GameHashes.BionicOffline, null, null)
				.ScheduleChange(null, GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Not(new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Transition.ConditionCallback(BionicBedTimeModeChore.IsBedTimeAllowed)))
				.ToggleWork("Defragmenting", new Action<BionicBedTimeModeChore.Instance>(BionicBedTimeModeChore.BeginWorkOnZone), (BionicBedTimeModeChore.Instance smi) => smi.GetAssignedDefragmentationZone() != null, this.end, null)
				.ToggleTag(GameTags.BionicBedTime);
			this.defragmentingWithoutAssignable.ParamTransition<GameObject>(this.defragmentationZone, this.approach, new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Parameter<GameObject>.Callback(BionicBedTimeModeChore.HasDefragmentationZoneAssignedAndReachable)).EventTransition(GameHashes.AssignableReachabilityChanged, this.approach, new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Transition.ConditionCallback(BionicBedTimeModeChore.HasDefragmentationZoneAssignedAndReachable)).ToggleAnims("anim_bionic_kanim", 0f)
				.ToggleTag(GameTags.BionicBedTime)
				.DefaultState(this.defragmentingWithoutAssignable.pre);
			this.defragmentingWithoutAssignable.pre.PlayAnim("low_power_pre").OnAnimQueueComplete(this.defragmentingWithoutAssignable.loop).ScheduleGoTo(1.5f, this.defragmentingWithoutAssignable.loop);
			this.defragmentingWithoutAssignable.loop.ScheduleChange(this.defragmentingWithoutAssignable.pst, GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Not(new StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Transition.ConditionCallback(BionicBedTimeModeChore.IsBedTimeAllowed))).EventTransition(GameHashes.BionicOffline, this.defragmentingWithoutAssignable.pst, null).PlayAnim("low_power_loop", KAnim.PlayMode.Loop);
			this.defragmentingWithoutAssignable.pst.PlayAnim("low_power_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(this.end);
			this.end.ReturnSuccess();
		}

		public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.ApproachSubState<IApproachable> approach;

		public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State defragmentingOnAssignable;

		public BionicBedTimeModeChore.States.DefragmentingStates defragmentingWithoutAssignable;

		public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State enter;

		public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State end;

		public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State unassigning;

		public StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.TargetParameter bionic;

		public StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.TargetParameter defragmentationZone;

		public StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Signal defragmentationZoneChangedSignal;

		public StateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.Signal defragmentationZoneUnassignined;

		public class DefragmentingStates : GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State
		{
			public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State pre;

			public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State loop;

			public GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.State pst;
		}
	}

	public class Instance : GameStateMachine<BionicBedTimeModeChore.States, BionicBedTimeModeChore.Instance, BionicBedTimeModeChore, object>.GameInstance
	{
		public DefragmentationZone GetAssignedDefragmentationZone()
		{
			return this.lastAssignedDefragmentationZone;
		}

		public Instance(BionicBedTimeModeChore master, GameObject duplicant)
			: base(master)
		{
			this.bedTimeMonitor = duplicant.GetSMI<BionicBedTimeMonitor.Instance>();
			base.sm.bionic.Set(duplicant, this, false);
			this.ownables = base.GetComponent<MinionIdentity>().GetSoleOwner();
			base.gameObject.Subscribe(-1585839766, new Action<object>(this.UpdateAssignedDefragmentationZone));
			this.UpdateAssignedDefragmentationZone(null);
		}

		protected override void OnCleanUp()
		{
			base.gameObject.Unsubscribe(-1585839766, new Action<object>(this.UpdateAssignedDefragmentationZone));
			base.OnCleanUp();
		}

		public override void StartSM()
		{
			this.UpdateAssignedDefragmentationZone(null);
			base.StartSM();
		}

		public bool IsDefragmentationZoneReachable()
		{
			return base.GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>().IsReachable(Db.Get().AssignableSlots.Bed);
		}

		public void UpdateAssignedDefragmentationZone(object slotInstanceObject)
		{
			DefragmentationZone defragmentationZone = null;
			AssignableSlotInstance assignableSlotInstance = ((slotInstanceObject == null) ? null : ((AssignableSlotInstance)slotInstanceObject));
			Assignable assignable = this.ownables.GetAssignable(Db.Get().AssignableSlots.Bed);
			if (assignableSlotInstance != null && assignableSlotInstance.IsUnassigning())
			{
				base.sm.defragmentationZoneUnassignined.Trigger(this);
				return;
			}
			if (assignable == null)
			{
				assignable = this.ownables.AutoAssignSlot(Db.Get().AssignableSlots.Bed);
			}
			if (assignable != null)
			{
				defragmentationZone = assignable.GetComponent<DefragmentationZone>();
			}
			if (this.lastAssignedDefragmentationZone != defragmentationZone)
			{
				AssignableReachabilitySensor sensor = base.GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>();
				if (sensor.IsEnabled)
				{
					sensor.Update();
				}
				this.lastAssignedDefragmentationZone = defragmentationZone;
				base.sm.defragmentationZone.Set(this.lastAssignedDefragmentationZone, this);
				base.sm.defragmentationZoneChangedSignal.Trigger(this);
			}
		}

		public BionicBedTimeMonitor.Instance bedTimeMonitor;

		private DefragmentationZone lastAssignedDefragmentationZone;

		private Ownables ownables;
	}
}
