using System;

public class CellCostQuery : PathFinderQuery
{
	public int resultCost { get; private set; }

	public void Reset(int target_cell, int max_cost)
	{
		this.targetCell = target_cell;
		this.maxCost = max_cost;
		this.resultCost = -1;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (cost > this.maxCost)
		{
			return true;
		}
		if (cell == this.targetCell)
		{
			this.resultCost = cost;
			return true;
		}
		return false;
	}

	private int targetCell;

	private int maxCost;
}
