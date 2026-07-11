using System;

public class SafeCellQuery : PathFinderQuery
{
	public SafeCellQuery Reset(MinionBrain brain, bool avoid_light)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetCellFlags = (SafeCellQuery.SafeFlags)0;
		this.avoid_light = avoid_light;
		return this;
	}

	public static SafeCellQuery.SafeFlags GetFlags(int cell, MinionBrain brain, bool avoid_light = false)
	{
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return (SafeCellQuery.SafeFlags)0;
		}
		bool flag = Grid.Solid[cell] || Grid.Solid[num];
		if (flag)
		{
			return (SafeCellQuery.SafeFlags)0;
		}
		bool flag2 = Grid.IsTileUnderConstruction[cell] || Grid.IsTileUnderConstruction[num];
		if (flag2)
		{
			return (SafeCellQuery.SafeFlags)0;
		}
		bool flag3 = brain.IsCellClear(cell);
		bool flag4 = !Grid.Element[cell].IsLiquid;
		bool flag5 = !Grid.Element[num].IsLiquid;
		bool flag6 = Grid.Temperature[cell] > 285.15f && Grid.Temperature[cell] < 303.15f;
		bool flag7 = brain.OxygenBreather.IsBreathableElementAtCell(cell, Grid.DefaultOffset);
		bool flag8 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole);
		bool flag9 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Tube);
		bool flag10 = !avoid_light || SleepChore.IsLightLevelOk(cell);
		if (cell == Grid.PosToCell(brain))
		{
			flag7 = !brain.OxygenBreather.IsSuffocating;
		}
		SafeCellQuery.SafeFlags safeFlags = (SafeCellQuery.SafeFlags)0;
		if (flag3)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsClear;
		}
		if (flag6)
		{
			safeFlags |= SafeCellQuery.SafeFlags.CorrectTemperature;
		}
		if (flag7)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsBreathable;
		}
		if (flag8)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLadder;
		}
		if (flag9)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotTube;
		}
		if (flag4)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLiquid;
		}
		if (flag5)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsNotLiquidOnMyFace;
		}
		if (flag10)
		{
			safeFlags |= SafeCellQuery.SafeFlags.IsLightOk;
		}
		return safeFlags;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain, this.avoid_light);
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

	private bool avoid_light;

	public enum SafeFlags
	{
		IsClear = 1,
		IsLightOk,
		IsNotLadder = 4,
		IsNotTube = 8,
		CorrectTemperature = 16,
		IsBreathable = 32,
		IsNotLiquidOnMyFace = 64,
		IsNotLiquid = 128
	}
}
