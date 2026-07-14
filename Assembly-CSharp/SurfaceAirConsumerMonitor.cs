using System;

public class SurfaceAirConsumerMonitor : GameStateMachine<SurfaceAirConsumerMonitor, SurfaceAirConsumerMonitor.Instance, IStateMachineTarget, SurfaceAirConsumerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.looking;
		this.looking.PreBrainUpdate(delegate(SurfaceAirConsumerMonitor.Instance smi)
		{
			smi.FindSurfaceCell();
		}).ToggleBehaviour(GameTags.Creatures.WantsToConsumeAir, (SurfaceAirConsumerMonitor.Instance smi) => smi.targetCell != Grid.InvalidCell, delegate(SurfaceAirConsumerMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
		this.cooldown.Enter(delegate(SurfaceAirConsumerMonitor.Instance smi)
		{
			smi.targetCell = Grid.InvalidCell;
		}).ScheduleGoTo((SurfaceAirConsumerMonitor.Instance smi) => smi.def.cooldown, this.looking);
	}

	public GameStateMachine<SurfaceAirConsumerMonitor, SurfaceAirConsumerMonitor.Instance, IStateMachineTarget, SurfaceAirConsumerMonitor.Def>.State cooldown;

	public GameStateMachine<SurfaceAirConsumerMonitor, SurfaceAirConsumerMonitor.Instance, IStateMachineTarget, SurfaceAirConsumerMonitor.Def>.State looking;

	public class Def : StateMachine.BaseDef
	{
		public SimHashes element = SimHashes.Oxygen;

		public float minimumMassThreshold = 0.2f;

		public float cooldown = 600f;
	}

	public new class Instance : GameStateMachine<SurfaceAirConsumerMonitor, SurfaceAirConsumerMonitor.Instance, IStateMachineTarget, SurfaceAirConsumerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SurfaceAirConsumerMonitor.Def def)
			: base(master, def)
		{
			this.navigator = master.GetComponent<Navigator>();
			this.prefabID = master.GetComponent<KPrefabID>();
		}

		public void FindSurfaceCell()
		{
			if (this.prefabID.HasTag(GameTags.Creatures.WantsToConsumeAir) && this.targetCell != Grid.InvalidCell)
			{
				return;
			}
			this.targetCell = Grid.InvalidCell;
			SurfaceAirConsumerMonitor.SurfaceCellQuery surfaceCellQuery = new SurfaceAirConsumerMonitor.SurfaceCellQuery(this, 25);
			this.navigator.RunQuery(surfaceCellQuery);
			if (surfaceCellQuery.success)
			{
				this.targetCell = surfaceCellQuery.GetResultCell();
			}
		}

		public bool IsSurfaceLiquidCell(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (!Grid.Element[cell].IsLiquid)
			{
				return false;
			}
			int num = Grid.CellAbove(cell);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.Element[num].id != base.def.element)
			{
				return false;
			}
			if (Grid.Mass[num] < base.def.minimumMassThreshold)
			{
				return false;
			}
			int num2 = Grid.CellBelow(cell);
			return Grid.IsValidCell(num2) && Grid.Element[num2].IsLiquid;
		}

		public int targetCell = Grid.InvalidCell;

		private Navigator navigator;

		private KPrefabID prefabID;
	}

	public class SurfaceCellQuery : PathFinderQuery
	{
		public SurfaceCellQuery(SurfaceAirConsumerMonitor.Instance smi, int maxIterations)
		{
			this.smi = smi;
			this.maxIterations = maxIterations;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			this.success = this.smi.IsSurfaceLiquidCell(cell);
			if (!this.success)
			{
				int num = this.maxIterations - 1;
				this.maxIterations = num;
				return num <= 0;
			}
			return true;
		}

		public bool success;

		private SurfaceAirConsumerMonitor.Instance smi;

		private int maxIterations;
	}
}
