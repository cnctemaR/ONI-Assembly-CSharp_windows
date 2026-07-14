using System;
using UnityEngine;

public class PunchClamOpenStates : GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.initialize;
		this.initialize.ParamTransition<GameObject>(this.clamTarget, this.exit, GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.IsNull).GoTo(this.approach);
		this.approach.Target(this.masterTarget).ParamTransition<GameObject>(this.clamTarget, this.exit, GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.IsNull).ToggleMainStatusItem(Db.Get().CreatureStatusItems.PunchClamApproach, null)
			.MoveTo(new Func<PunchClamOpenStates.Instance, int>(PunchClamOpenStates.GetBestCell), this.punch, this.exit, false)
			.Target(this.clamTarget)
			.EventHandlerTransition(GameHashes.WorkableCompleteWork, this.exit, new Func<PunchClamOpenStates.Instance, object, bool>(PunchClamOpenStates.CanNoLongerOpenClam));
		this.punch.ParamTransition<GameObject>(this.clamTarget, this.exit, GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.IsNull).DefaultState(this.punch.pre).ToggleMainStatusItem(Db.Get().CreatureStatusItems.PunchClamAttack, null);
		this.punch.pre.Face(this.clamTarget, 0f).PlayAnim((PunchClamOpenStates.Instance smi) => smi.def.PUNH_ANIM_PRE_NAME, KAnim.PlayMode.Once).OnAnimQueueComplete(this.punch.loop);
		this.punch.loop.PlayAnim((PunchClamOpenStates.Instance smi) => smi.def.PUNH_ANIM_LOOP_NAME, KAnim.PlayMode.Loop).ScheduleGoTo(2f, this.punch.pst);
		this.punch.pst.PlayAnim((PunchClamOpenStates.Instance smi) => smi.def.PUNH_ANIM_PST_NAME, KAnim.PlayMode.Once).OnAnimQueueComplete(this.openClam);
		this.openClam.Enter(new StateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.State.Callback(PunchClamOpenStates.OpenClam)).GoTo(this.exit);
		this.exit.BehaviourComplete(GameTags.Creatures.WantsToPunchClam, false);
	}

	public static int GetBestCell(PunchClamOpenStates.Instance smi)
	{
		return smi.GetBestCellToStand();
	}

	public static void OpenClam(PunchClamOpenStates.Instance smi)
	{
		smi.OpenClam();
	}

	public static bool CanNoLongerOpenClam(PunchClamOpenStates.Instance smi, object o)
	{
		return smi.clam != null && !smi.clam.IsClosedAndReadyForHarvesting;
	}

	public GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.State initialize;

	public GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.State approach;

	public GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.PreLoopPostState punch;

	public GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.State openClam;

	public GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.State exit;

	private StateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.TargetParameter clamTarget;

	public class Def : StateMachine.BaseDef
	{
		public string PUNH_ANIM_PRE_NAME = "slap_pre";

		public string PUNH_ANIM_LOOP_NAME = "slap";

		public string PUNH_ANIM_PST_NAME = "slap_pst";
	}

	public new class Instance : GameStateMachine<PunchClamOpenStates, PunchClamOpenStates.Instance, IStateMachineTarget, PunchClamOpenStates.Def>.GameInstance
	{
		public ClamHarvestable clam
		{
			get
			{
				if (!(base.sm.clamTarget.Get(this) == null))
				{
					return base.sm.clamTarget.Get(this).GetComponent<ClamHarvestable>();
				}
				return null;
			}
		}

		public Instance(Chore<PunchClamOpenStates.Instance> chore, PunchClamOpenStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToPunchClam);
			this.navigator = base.GetComponent<Navigator>();
			this.punchClamMonitor = base.gameObject.GetSMI<PunchClamMonitor.Instance>();
		}

		public override void StartSM()
		{
			base.sm.clamTarget.Set(this.punchClamMonitor.Clam, this);
			base.StartSM();
		}

		public int GetBestCellToStand()
		{
			ClamHarvestable clam = this.clam;
			if (clam == null)
			{
				return Grid.InvalidCell;
			}
			int num = Grid.PosToCell(clam.transform.GetPosition());
			CellOffset[] clamCellOffsets = PunchClamMonitor.ClamCellOffsets;
			float num2 = float.MaxValue;
			int num3 = Grid.InvalidCell;
			foreach (CellOffset cellOffset in clamCellOffsets)
			{
				int num4 = Grid.OffsetCell(num, cellOffset);
				int navigationCost = this.navigator.GetNavigationCost(num4);
				if (navigationCost != -1 && (float)navigationCost < num2)
				{
					num2 = (float)navigationCost;
					num3 = num4;
				}
			}
			return num3;
		}

		public void OpenClam()
		{
			if (this.clam != null)
			{
				this.clam.PunchOpen();
			}
		}

		public PunchClamMonitor.Instance punchClamMonitor;

		private Navigator navigator;
	}
}
