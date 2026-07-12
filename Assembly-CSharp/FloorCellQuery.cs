using System;
using System.Collections.Generic;

public class FloorCellQuery : PathFinderQuery
{
	public FloorCellQuery Reset(int max_results, int adjacent_cells_buffer = 0)
	{
		this.max_results = max_results;
		this.adjacent_cells_buffer = adjacent_cells_buffer;
		this.result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!this.result_cells.Contains(cell) && this.CheckValidFloorCell(cell))
		{
			this.result_cells.Add(cell);
		}
		return this.result_cells.Count >= this.max_results;
	}

	private bool CheckValidFloorCell(int testCell)
	{
		if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell))
		{
			return false;
		}
		int cellInDirection = Grid.GetCellInDirection(testCell, Direction.Up);
		int cellInDirection2 = Grid.GetCellInDirection(testCell, Direction.Down);
		if (!Grid.ObjectLayers[1].ContainsKey(testCell) && Grid.IsValidCell(cellInDirection2) && Grid.IsSolidCell(cellInDirection2) && Grid.IsValidCell(cellInDirection) && !Grid.IsSolidCell(cellInDirection))
		{
			int num = testCell;
			int num2 = testCell;
			for (int i = 0; i < this.adjacent_cells_buffer; i++)
			{
				num = Grid.CellLeft(num);
				num2 = Grid.CellRight(num2);
				if (!Grid.IsValidCell(num) || Grid.IsSolidCell(num))
				{
					return false;
				}
				if (!Grid.IsValidCell(num2) || Grid.IsSolidCell(num2))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public List<int> result_cells = new List<int>();

	private int max_results;

	private int adjacent_cells_buffer;
}
