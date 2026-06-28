using System;

public class CellQuery : PathFinderQuery
{
	public void Reset(int target_cell)
	{
		this.targetCell = target_cell;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		return cell == this.targetCell;
	}

	private int targetCell;
}
