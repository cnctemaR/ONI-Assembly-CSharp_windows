using System;

public class AbsorbCellQuery : PathFinderQuery
{
	public AbsorbCellQuery()
	{
		this.checker = Game.Instance.safetyConditions.AbsorbCellCellChecker;
	}

	public AbsorbCellQuery Reset(MinionBrain brain, bool prioritizeClosestCellOverOxygenMass)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetCellFlags = (AbsorbCellQuery.SafeFlags)0;
		this.targetBreathabilityScore = 0f;
		this.prioritizeClosestCellOverOxygenMass = prioritizeClosestCellOverOxygenMass;
		this.context = new SafetyChecker.Context(brain);
		ScaldingMonitor.Instance instance = ((brain == null) ? null : brain.GetSMI<ScaldingMonitor.Instance>());
		this.scaldingTreshold = ((instance == null) ? (-1f) : instance.GetScaldingThreshold());
		return this;
	}

	public static AbsorbCellQuery.SafeFlags GetFlags(int cell, MinionBrain brain, float scaldingTreshold, out float breathabilityScore)
	{
		breathabilityScore = 0f;
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return (AbsorbCellQuery.SafeFlags)0;
		}
		if (Grid.Solid[cell] || Grid.Solid[num])
		{
			return (AbsorbCellQuery.SafeFlags)0;
		}
		if (Grid.IsTileUnderConstruction[cell] || Grid.IsTileUnderConstruction[num])
		{
			return (AbsorbCellQuery.SafeFlags)0;
		}
		bool flag = brain.IsCellClear(cell);
		bool flag2 = !Grid.Element[cell].IsLiquid;
		bool flag3 = !Grid.Element[num].IsLiquid;
		bool flag4 = scaldingTreshold < 0f || Grid.Temperature[cell] < scaldingTreshold;
		bool flag5 = Grid.Radiation[cell] < 250f;
		bool flag6 = false;
		if (brain.OxygenBreather != null)
		{
			GasBreatherFromWorldProvider.BreathableCellData bestBreathableCellAroundSpecificCell = GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(cell, GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS, brain.OxygenBreather);
			flag6 = bestBreathableCellAroundSpecificCell.IsBreathable;
			if (flag6)
			{
				breathabilityScore = bestBreathableCellAroundSpecificCell.Mass;
			}
		}
		bool flag7 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole);
		bool flag8 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Tube);
		AbsorbCellQuery.SafeFlags safeFlags = (AbsorbCellQuery.SafeFlags)0;
		if (flag)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsClear;
		}
		if (flag4)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotScaldingTemperatures;
		}
		if (flag5)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotRadiated;
		}
		if (flag6)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsBreathable;
		}
		if (flag7)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotLadder;
		}
		if (flag8)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotTube;
		}
		if (flag2)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotLiquid;
		}
		if (flag3)
		{
			safeFlags |= AbsorbCellQuery.SafeFlags.IsNotLiquidOnMyFace;
		}
		return safeFlags;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		bool flag;
		this.checker.GetSafetyConditions(cell, cost, this.context, out flag);
		if (flag)
		{
			float num = 0f;
			AbsorbCellQuery.SafeFlags flags = AbsorbCellQuery.GetFlags(cell, this.brain, this.scaldingTreshold, out num);
			bool flag2 = flags > this.targetCellFlags;
			bool flag3 = flags == this.targetCellFlags && cost < this.targetCost;
			bool flag4 = flags == this.targetCellFlags && cost == this.targetCost;
			bool flag5 = num > this.targetBreathabilityScore;
			bool flag6 = flags == this.targetCellFlags && flag5;
			bool flag7;
			if (this.prioritizeClosestCellOverOxygenMass)
			{
				flag7 = flag2 || flag3 || (flag4 && flag5);
			}
			else
			{
				flag7 = flag2 || flag6 || flag3;
			}
			if (flag7)
			{
				this.targetBreathabilityScore = num;
				this.targetCellFlags = flags;
				this.targetCost = cost;
				this.targetCell = cell;
			}
		}
		return false;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private MinionBrain brain;

	private float scaldingTreshold = -1f;

	private int targetCell;

	private int targetCost;

	private bool prioritizeClosestCellOverOxygenMass;

	private float targetBreathabilityScore;

	public AbsorbCellQuery.SafeFlags targetCellFlags;

	public float targetCellBreathabilityScore;

	private SafetyChecker checker;

	private SafetyChecker.Context context;

	public enum SafeFlags
	{
		IsClear = 1,
		IsNotLadder,
		IsNotTube = 4,
		IsNotRadiated = 16,
		IsBreathable = 32,
		IsNotScaldingTemperatures = 64,
		IsNotLiquidOnMyFace = 128,
		IsNotLiquid = 256
	}
}
