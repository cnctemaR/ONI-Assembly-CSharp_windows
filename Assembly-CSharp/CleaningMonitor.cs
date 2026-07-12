using System;

public class CleaningMonitor : GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.clean;
		this.clean.ToggleBehaviour(GameTags.Creatures.Cleaning, (CleaningMonitor.Instance smi) => smi.CanCleanElementState(), delegate(CleaningMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
		this.cooldown.ScheduleGoTo((CleaningMonitor.Instance smi) => smi.def.coolDown, this.clean);
	}

	public GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.State cooldown;

	public GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.State clean;

	public class Def : StateMachine.BaseDef
	{
		public Element.State elementState = Element.State.Liquid;

		public CellOffset[] cellOffsets;

		public float coolDown = 30f;
	}

	public new class Instance : GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CleaningMonitor.Def def)
			: base(master, def)
		{
		}

		public bool CanCleanElementState()
		{
			int num = Grid.PosToCell(base.smi.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (!Grid.IsLiquid(num) && base.smi.def.elementState == Element.State.Liquid)
			{
				return false;
			}
			if (Grid.DiseaseCount[num] > 0)
			{
				return true;
			}
			if (base.smi.def.cellOffsets != null)
			{
				foreach (CellOffset cellOffset in base.smi.def.cellOffsets)
				{
					int num2 = Grid.OffsetCell(num, cellOffset);
					if (Grid.IsValidCell(num2) && Grid.DiseaseCount[num2] > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
