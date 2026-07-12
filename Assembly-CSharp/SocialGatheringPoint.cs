using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SocialGatheringPoint : StateMachineComponent<SocialGatheringPoint.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.workables = new SocialGatheringPointWorkable[this.choreOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[i]), Grid.SceneLayer.Move);
			SocialGatheringPointWorkable socialGatheringPointWorkable = ChoreHelpers.CreateLocator("SocialGatheringPointWorkable", vector).AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.basePriority = this.basePriority;
			socialGatheringPointWorkable.specificEffect = this.socialEffect;
			socialGatheringPointWorkable.OnWorkableEventCB = new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
			socialGatheringPointWorkable.SetWorkTime(this.workTime);
			this.workables[i] = socialGatheringPointWorkable;
		}
		this.tracker = new SocialChoreTracker(base.gameObject, this.choreOffsets);
		this.tracker.choreCount = this.choreCount;
		this.tracker.CreateChoreCB = new Func<int, Chore>(this.CreateChore);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (this.tracker != null)
		{
			this.tracker.Clear();
			this.tracker = null;
		}
		if (this.workables != null)
		{
			for (int i = 0; i < this.workables.Length; i++)
			{
				if (this.workables[i])
				{
					Util.KDestroyGameObject(this.workables[i]);
					this.workables[i] = null;
				}
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = this.workables[i];
		ChoreType relax = Db.Get().ChoreTypes.Relax;
		IStateMachineTarget stateMachineTarget = workable;
		ChoreProvider choreProvider = null;
		bool flag = true;
		Action<Chore> action = null;
		Action<Chore> action2 = null;
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		WorkChore<SocialGatheringPointWorkable> workChore = new WorkChore<SocialGatheringPointWorkable>(relax, stateMachineTarget, choreProvider, flag, action, action2, new Action<Chore>(this.OnSocialChoreEnd), false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, false);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		workChore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, workable);
		return workChore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.smi.IsInsideState(base.smi.sm.on))
		{
			this.tracker.Update(true);
		}
	}

	private void OnWorkableEvent(Workable workable, Workable.WorkableEvent workable_event)
	{
		if (workable_event == Workable.WorkableEvent.WorkStarted)
		{
			if (this.OnSocializeBeginCB != null)
			{
				this.OnSocializeBeginCB();
				return;
			}
		}
		else if (workable_event == Workable.WorkableEvent.WorkStopped && this.OnSocializeEndCB != null)
		{
			this.OnSocializeEndCB();
		}
	}

	public CellOffset[] choreOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public int basePriority;

	public string socialEffect;

	public float workTime = 15f;

	public global::System.Action OnSocializeBeginCB;

	public global::System.Action OnSocializeEndCB;

	private SocialChoreTracker tracker;

	private SocialGatheringPointWorkable[] workables;

	public class States : GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.root.DoNothing();
			this.off.TagTransition(GameTags.Operational, this.on, false);
			this.on.TagTransition(GameTags.Operational, this.off, true).Enter("CreateChore", delegate(SocialGatheringPoint.StatesInstance smi)
			{
				smi.master.tracker.Update(true);
			}).Exit("CancelChore", delegate(SocialGatheringPoint.StatesInstance smi)
			{
				smi.master.tracker.Update(false);
			});
		}

		public GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State off;

		public GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State on;
	}

	public class StatesInstance : GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.GameInstance
	{
		public StatesInstance(SocialGatheringPoint smi)
			: base(smi)
		{
		}
	}
}
