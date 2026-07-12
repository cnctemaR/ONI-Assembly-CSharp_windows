using System;
using System.Collections.Generic;
using UnityEngine;

public class StaterpillarCellQuery : PathFinderQuery
{
	public StaterpillarCellQuery Reset(int max_results, GameObject tester)
	{
		this.max_results = max_results;
		this.tester = tester;
		this.result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!this.result_cells.Contains(cell) && this.CheckValidRoofCell(cell))
		{
			this.result_cells.Add(cell);
		}
		return this.result_cells.Count >= this.max_results;
	}

	private bool CheckValidRoofCell(int testCell)
	{
		if (!this.tester.GetComponent<Navigator>().NavGrid.NavTable.IsValid(testCell, NavType.Ceiling))
		{
			return false;
		}
		int cellInDirection = Grid.GetCellInDirection(testCell, Direction.Down);
		return !Grid.ObjectLayers[1].ContainsKey(testCell) && !Grid.ObjectLayers[1].ContainsKey(cellInDirection) && !Grid.Objects[cellInDirection, 29] && Grid.IsValidBuildingCell(testCell) && !Grid.IsLiquid(testCell) && Grid.IsValidCell(cellInDirection) && Grid.IsValidBuildingCell(cellInDirection) && !Grid.IsSolidCell(cellInDirection) && !Grid.IsLiquid(cellInDirection);
	}

	public List<int> result_cells = new List<int>();

	private int max_results;

	private GameObject tester;
}
