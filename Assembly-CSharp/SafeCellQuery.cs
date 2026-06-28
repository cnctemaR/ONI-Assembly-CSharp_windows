using System;

public class SafeCellQuery : PathFinderQuery
{
	public SafeCellQuery Reset(MinionBrain brain)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetCellFlags = (SafeCellQuery.SafeFlags)0;
		return this;
	}

	public static SafeCellQuery.SafeFlags GetFlags(int cell, MinionBrain brain)
	{
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return (SafeCellQuery.SafeFlags)0;
		}
		bool flag = brain.IsCellClear(cell);
		bool flag2 = !Grid.Element[cell].IsLiquid;
		bool flag3 = !Grid.Element[num].IsLiquid;
		bool flag4 = Grid.Foundation[cell] || (Grid.IsValidCell(num) && Grid.Foundation[num]);
		bool flag5 = Grid.Temperature[cell] < 303f;
		bool flag6 = brain.OxygenBreather.IsBreathableElementAtCell(cell, Grid.DefaultOffset);
		bool flag7 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder);
		bool flag8 = Grid.IsTileUnderConstruction[cell] || (Grid.IsValidCell(num) && Grid.IsTileUnderConstruction[num]);
		if (cell == Grid.PosToCell(brain))
		{
			flag6 = !brain.OxygenBreather.IsSuffocating;
		}
		SafeCellQuery.SafeFlags safeFlags = (SafeCellQuery.SafeFlags)0;
		if (flag)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsClear;
		}
		if (flag5)
		{
			safeFlags |= SafeCellQuery.SafeFlags.CorrectTemperature;
		}
		if (flag6)
		{
			safeFlags |= SafeCellQuery.SafeFlags.HasSomeOxygen;
		}
		if (flag7)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLadder;
		}
		if (flag2)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLiquid;
		}
		if (flag3)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLiquidOnMyFace;
		}
		if (flag4 || flag8)
		{
			safeFlags = (SafeCellQuery.SafeFlags)0;
		}
		return safeFlags;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain);
		bool flag = flags > this.targetCellFlags;
		bool flag2 = flags == this.targetCellFlags && cost < this.targetCost;
		if (flag || flag2)
		{
			this.targetCellFlags = flags;
			this.targetCost = cost;
			this.targetCell = cell;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private MinionBrain brain;

	private int targetCell;

	private int targetCost;

	public SafeCellQuery.SafeFlags targetCellFlags;

	public enum SafeFlags
	{
		IsNotLiquid = 1,
		IsNotLadder,
		IsNotLiquidOnMyFace = 4,
		CorrectTemperature = 8,
		HasSomeOxygen = 16,
		HasLotsOxygen = 32,
		IsClear = 64
	}
}
