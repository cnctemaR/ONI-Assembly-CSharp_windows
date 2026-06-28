using System;

public class CellCostQuery : PathFinderQuery
{
	public int resultCost { get; private set; }

	public void Reset(int target_cell, int max_cost)
	{
		this.targetCell = target_cell;
		this.maxCost = max_cost;
		this.resultCost = PathProber.InvalidCost;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		bool flag;
		if (cost > this.maxCost)
		{
			flag = true;
		}
		else if (cell == this.targetCell)
		{
			this.resultCost = cost;
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private int targetCell;

	private int maxCost;
}
