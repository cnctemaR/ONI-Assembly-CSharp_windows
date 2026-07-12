using System;
using System.Collections.Generic;

public class WarmthProvider : GameStateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>
{
	public static bool IsWarmCell(int cell)
	{
		return WarmthProvider.WarmCells.ContainsKey(cell) && WarmthProvider.WarmCells[cell] > 0;
	}

	public static int GetWarmthValue(int cell)
	{
		if (!WarmthProvider.WarmCells.ContainsKey(cell))
		{
			return -1;
		}
		return (int)WarmthProvider.WarmCells[cell];
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.off;
		this.off.EventTransition(GameHashes.ActiveChanged, this.on, (WarmthProvider.Instance smi) => smi.GetComponent<Operational>().IsActive).Enter(new StateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>.State.Callback(WarmthProvider.RemoveWarmCells));
		this.on.EventTransition(GameHashes.ActiveChanged, this.off, (WarmthProvider.Instance smi) => !smi.GetComponent<Operational>().IsActive).TagTransition(GameTags.Operational, this.off, true).Enter(new StateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>.State.Callback(WarmthProvider.AddWarmCells));
	}

	private static void AddWarmCells(WarmthProvider.Instance smi)
	{
		smi.AddWarmCells();
	}

	private static void RemoveWarmCells(WarmthProvider.Instance smi)
	{
		smi.RemoveWarmCells();
	}

	public static Dictionary<int, byte> WarmCells = new Dictionary<int, byte>();

	public GameStateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>.State off;

	public GameStateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>.State on;

	public class Def : StateMachine.BaseDef
	{
		public Vector2I OriginOffset;

		public Vector2I RangeMin;

		public Vector2I RangeMax;

		public Func<int, bool> blockingCellCallback = new Func<int, bool>(Grid.IsSolidCell);
	}

	public new class Instance : GameStateMachine<WarmthProvider, WarmthProvider.Instance, IStateMachineTarget, WarmthProvider.Def>.GameInstance
	{
		public bool IsWarming
		{
			get
			{
				return base.IsInsideState(base.sm.on);
			}
		}

		public Instance(IStateMachineTarget master, WarmthProvider.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			EntityCellVisualizer component = base.GetComponent<EntityCellVisualizer>();
			if (component != null)
			{
				component.AddPort(EntityCellVisualizer.Ports.HeatSource, default(CellOffset));
			}
			this.WorldID = base.gameObject.GetMyWorldId();
			this.SetupRange();
			this.CreateCellListeners();
			base.StartSM();
		}

		private void SetupRange()
		{
			Vector2I vector2I = Grid.PosToXY(base.transform.GetPosition());
			Vector2I vector2I2 = base.def.OriginOffset;
			this.range_min = base.def.RangeMin;
			this.range_max = base.def.RangeMax;
			Rotatable rotatable;
			if (base.gameObject.TryGetComponent<Rotatable>(out rotatable))
			{
				vector2I2 = rotatable.GetRotatedOffset(vector2I2);
				Vector2I rotatedOffset = rotatable.GetRotatedOffset(this.range_min);
				Vector2I rotatedOffset2 = rotatable.GetRotatedOffset(this.range_max);
				this.range_min.x = ((rotatedOffset.x < rotatedOffset2.x) ? rotatedOffset.x : rotatedOffset2.x);
				this.range_min.y = ((rotatedOffset.y < rotatedOffset2.y) ? rotatedOffset.y : rotatedOffset2.y);
				this.range_max.x = ((rotatedOffset.x > rotatedOffset2.x) ? rotatedOffset.x : rotatedOffset2.x);
				this.range_max.y = ((rotatedOffset.y > rotatedOffset2.y) ? rotatedOffset.y : rotatedOffset2.y);
			}
			this.origin = vector2I + vector2I2;
		}

		public bool ContainsCell(int cell)
		{
			if (this.cellsInRange == null)
			{
				return false;
			}
			for (int i = 0; i < this.cellsInRange.Length; i++)
			{
				if (this.cellsInRange[i] == cell)
				{
					return true;
				}
			}
			return false;
		}

		private void UnmarkAllCellsInRange()
		{
			if (this.cellsInRange != null)
			{
				for (int i = 0; i < this.cellsInRange.Length; i++)
				{
					int num = this.cellsInRange[i];
					if (WarmthProvider.WarmCells.ContainsKey(num))
					{
						Dictionary<int, byte> warmCells = WarmthProvider.WarmCells;
						int num2 = num;
						byte b = warmCells[num2];
						warmCells[num2] = b - 1;
					}
				}
			}
			this.cellsInRange = null;
		}

		private void UpdateCellsInRange()
		{
			this.UnmarkAllCellsInRange();
			Grid.PosToCell(this);
			List<int> list = new List<int>();
			for (int i = 0; i <= this.range_max.y - this.range_min.y; i++)
			{
				int num = this.origin.y + this.range_min.y + i;
				for (int j = 0; j <= this.range_max.x - this.range_min.x; j++)
				{
					int num2 = Grid.XYToCell(this.origin.x + this.range_min.x + j, num);
					if (Grid.IsValidCellInWorld(num2, this.WorldID) && this.IsCellVisible(num2))
					{
						list.Add(num2);
						if (!WarmthProvider.WarmCells.ContainsKey(num2))
						{
							WarmthProvider.WarmCells.Add(num2, 0);
						}
						Dictionary<int, byte> warmCells = WarmthProvider.WarmCells;
						int num3 = num2;
						byte b = warmCells[num3];
						warmCells[num3] = b + 1;
					}
				}
			}
			this.cellsInRange = list.ToArray();
		}

		public void AddWarmCells()
		{
			this.UpdateCellsInRange();
		}

		public void RemoveWarmCells()
		{
			this.UnmarkAllCellsInRange();
		}

		protected override void OnCleanUp()
		{
			this.RemoveWarmCells();
			this.ClearCellListeners();
			base.OnCleanUp();
		}

		public bool IsCellVisible(int cell)
		{
			Vector2I vector2I = Grid.CellToXY(Grid.PosToCell(this));
			Vector2I vector2I2 = Grid.CellToXY(cell);
			return Grid.TestLineOfSight(vector2I.x, vector2I.y, vector2I2.x, vector2I2.y, base.def.blockingCellCallback, false, false);
		}

		public void OnSolidCellChanged(object obj)
		{
			if (this.IsWarming)
			{
				this.UpdateCellsInRange();
			}
		}

		private void CreateCellListeners()
		{
			Grid.PosToCell(this);
			List<HandleVector<int>.Handle> list = new List<HandleVector<int>.Handle>();
			for (int i = 0; i <= this.range_max.y - this.range_min.y; i++)
			{
				int num = this.origin.y + this.range_min.y + i;
				for (int j = 0; j <= this.range_max.x - this.range_min.x; j++)
				{
					int num2 = Grid.XYToCell(this.origin.x + this.range_min.x + j, num);
					if (Grid.IsValidCellInWorld(num2, this.WorldID))
					{
						list.Add(GameScenePartitioner.Instance.Add("WarmthProvider Visibility", base.gameObject, num2, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidCellChanged)));
					}
				}
			}
			this.partitionEntries = list.ToArray();
		}

		private void ClearCellListeners()
		{
			if (this.partitionEntries != null)
			{
				for (int i = 0; i < this.partitionEntries.Length; i++)
				{
					HandleVector<int>.Handle handle = this.partitionEntries[i];
					GameScenePartitioner.Instance.Free(ref handle);
				}
			}
		}

		public int WorldID;

		private int[] cellsInRange;

		private HandleVector<int>.Handle[] partitionEntries;

		public Vector2I range_min;

		public Vector2I range_max;

		public Vector2I origin;
	}
}
