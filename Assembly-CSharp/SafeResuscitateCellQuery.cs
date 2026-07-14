using System;

public class SafeResuscitateCellQuery : PathFinderQuery
{
	public SafeResuscitateCellQuery Reset(OxygenBreather oxygen_breather)
	{
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.oxygenBreather = oxygen_breather;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (Grid.Solid[cell] || Grid.Solid[num])
		{
			return false;
		}
		int num2 = Grid.CellBelow(cell);
		if (!Grid.IsValidCell(num2) || !Grid.Solid[num2])
		{
			return false;
		}
		if (Grid.IsSubstantialLiquid(cell, 0.35f) || Grid.Element[num].IsLiquid)
		{
			return false;
		}
		if (this.oxygenBreather != null && !GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(cell, new CellOffset[]
		{
			CellOffset.none,
			CellOffset.up
		}, this.oxygenBreather).IsBreathable)
		{
			return false;
		}
		if (cost < this.targetCost)
		{
			this.targetCost = cost;
			this.targetCell = cell;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private int targetCell;

	private int targetCost;

	private OxygenBreather oxygenBreather;
}
