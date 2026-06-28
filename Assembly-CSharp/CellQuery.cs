using System;

public class CellQuery : PathFinderQuery
{
	public CellQuery Reset(int target_cell)
	{
		this.targetCell = target_cell;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		return cell == this.targetCell;
	}

	private int targetCell;
}
