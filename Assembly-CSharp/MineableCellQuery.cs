using System;
using System.Collections.Generic;

public class MineableCellQuery : PathFinderQuery
{
	public MineableCellQuery Reset(Tag element, int max_results)
	{
		this.element = element;
		this.max_results = max_results;
		this.result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!this.result_cells.Contains(cell) && this.CheckValidMineCell(this.element, cell))
		{
			this.result_cells.Add(cell);
		}
		return this.result_cells.Count >= this.max_results;
	}

	private bool CheckValidMineCell(Tag element, int testCell)
	{
		if (!Grid.IsValidCell(testCell))
		{
			return false;
		}
		foreach (Direction direction in MineableCellQuery.DIRECTION_CHECKS)
		{
			int cellInDirection = Grid.GetCellInDirection(testCell, direction);
			if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && !Grid.Foundation[cellInDirection] && Grid.Element[cellInDirection].tag == element)
			{
				return true;
			}
		}
		return false;
	}

	public List<int> result_cells = new List<int>();

	private Tag element;

	private int max_results;

	public static List<Direction> DIRECTION_CHECKS = new List<Direction>
	{
		Direction.Down,
		Direction.Right,
		Direction.Left,
		Direction.Up
	};
}
