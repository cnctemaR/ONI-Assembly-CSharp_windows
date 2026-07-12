using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementQuery : PathFinderQuery
{
	public BuildingPlacementQuery Reset(int max_results, GameObject toPlace)
	{
		this.max_results = max_results;
		this.toPlace = toPlace;
		this.cellOffsets = toPlace.GetComponent<OccupyArea>().OccupiedCellsOffsets;
		this.result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!this.result_cells.Contains(cell) && this.CheckValidPlaceCell(cell))
		{
			this.result_cells.Add(cell);
		}
		return this.result_cells.Count >= this.max_results;
	}

	private bool CheckValidPlaceCell(int testCell)
	{
		if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell) || Grid.ObjectLayers[1].ContainsKey(testCell))
		{
			return false;
		}
		bool flag = true;
		int widthInCells = this.toPlace.GetComponent<OccupyArea>().GetWidthInCells();
		int num = testCell;
		for (int i = 0; i < widthInCells; i++)
		{
			int cellInDirection = Grid.GetCellInDirection(num, Direction.Down);
			if (!Grid.IsValidCell(cellInDirection) || !Grid.IsSolidCell(cellInDirection))
			{
				flag = false;
				break;
			}
			num = Grid.GetCellInDirection(num, Direction.Right);
		}
		if (flag)
		{
			for (int j = 0; j < this.cellOffsets.Length; j++)
			{
				CellOffset cellOffset = this.cellOffsets[j];
				int num2 = Grid.OffsetCell(testCell, cellOffset);
				if (!Grid.IsValidCell(num2) || Grid.IsSolidCell(num2) || !Grid.IsValidBuildingCell(num2) || Grid.ObjectLayers[1].ContainsKey(num2))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	public List<int> result_cells = new List<int>();

	private int max_results;

	private GameObject toPlace;

	private CellOffset[] cellOffsets;
}
