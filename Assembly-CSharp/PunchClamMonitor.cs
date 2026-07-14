using System;
using UnityEngine;

public class PunchClamMonitor : GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Never;
		default_state = this.searching;
		this.searching.ParamTransition<GameObject>(this.clamTarget, this.found, GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.IsNotNull).Update(new Action<PunchClamMonitor.Instance, float>(PunchClamMonitor.SearchUpdate), UpdateRate.SIM_4000ms, false);
		this.found.ParamTransition<GameObject>(this.clamTarget, this.searching, GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.IsNull).Toggle("Toggle clam reservation", new StateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.State.Callback(PunchClamMonitor.ReserveClam), new StateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.State.Callback(PunchClamMonitor.UnreserveClam)).ToggleBehaviour(GameTags.Creatures.WantsToPunchClam, (PunchClamMonitor.Instance smi) => true, new Action<PunchClamMonitor.Instance>(PunchClamMonitor.ClearTarget));
	}

	public static void SearchUpdate(PunchClamMonitor.Instance smi, float dt)
	{
		smi.SearchForClam(dt);
	}

	public static void ReserveClam(PunchClamMonitor.Instance smi)
	{
		smi.ToggleClamReservation(true);
	}

	public static void UnreserveClam(PunchClamMonitor.Instance smi)
	{
		smi.ToggleClamReservation(false);
	}

	private static void ClearTarget(PunchClamMonitor.Instance smi)
	{
		smi.sm.clamTarget.Set(null, smi);
	}

	public static Tag ReservedTag = new Tag("PunchClamReserved");

	public static CellOffset[] ClamCellOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	public GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.State searching;

	public GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.State found;

	private StateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.TargetParameter clamTarget;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<PunchClamMonitor, PunchClamMonitor.Instance, IStateMachineTarget, PunchClamMonitor.Def>.GameInstance
	{
		public ClamHarvestable Clam
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

		public Instance(IStateMachineTarget master, PunchClamMonitor.Def def)
			: base(master, def)
		{
			this.navigator = base.GetComponent<Navigator>();
		}

		public void SearchForClam(float dt)
		{
			Grid.PosToCell(this);
			foreach (object obj in Components.ClamHarvestables)
			{
				ClamHarvestable clamHarvestable = (ClamHarvestable)obj;
				if (!(clamHarvestable == null) && clamHarvestable.IsClosedAndReadyForHarvesting && !clamHarvestable.HasTag(PunchClamMonitor.ReservedTag) && this.CanReachClam(clamHarvestable))
				{
					base.sm.clamTarget.Set(clamHarvestable.gameObject, this, false);
					break;
				}
			}
		}

		public void ToggleClamReservation(bool reserve)
		{
			if (!reserve)
			{
				if (this.reservedClamID != null)
				{
					this.reservedClamID.RemoveTag(PunchClamMonitor.ReservedTag);
					this.reservedClamID = null;
				}
				return;
			}
			ClamHarvestable clam = this.Clam;
			if (clam == null)
			{
				return;
			}
			this.reservedClamID = clam.GetComponent<KPrefabID>();
			this.reservedClamID.AddTag(PunchClamMonitor.ReservedTag, false);
		}

		public bool CanReachClam(ClamHarvestable targetClam)
		{
			if (Vector2.Distance(base.smi.transform.position, targetClam.transform.position) > 32f)
			{
				return false;
			}
			int num = Grid.PosToCell(targetClam.transform.GetPosition());
			foreach (CellOffset cellOffset in PunchClamMonitor.ClamCellOffsets)
			{
				int num2 = Grid.OffsetCell(num, cellOffset);
				if (this.navigator.GetNavigationCost(num2) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private KPrefabID reservedClamID;

		private Navigator navigator;
	}
}
