using System;
using STRINGS;
using UnityEngine;

public class ConditionFlightPathIsClear : RocketFlightCondition
{
	public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
	{
		this.module = module;
		this.bufferWidth = bufferWidth;
	}

	public override bool EvaluateFlightCondition()
	{
		this.Update();
		return this.hasClearSky;
	}

	public override StatusItem GetFailureStatusItem()
	{
		return Db.Get().BuildingStatusItems.PathNotClear;
	}

	public void Update()
	{
		Extents extents = this.module.GetComponent<Building>().GetExtents();
		int num = extents.x - this.bufferWidth;
		int num2 = extents.x + extents.width - 1 + this.bufferWidth;
		int y = extents.y;
		int num3 = Grid.XYToCell(num, y);
		int num4 = Grid.XYToCell(num2, y);
		this.hasClearSky = true;
		this.obstructedTile = -1;
		for (int i = num3; i <= num4; i++)
		{
			if (!this.CanReachSpace(i))
			{
				this.hasClearSky = false;
				return;
			}
		}
	}

	private bool CanReachSpace(int startCell)
	{
		int num = startCell;
		while (Grid.CellRow(num) < Grid.HeightInCells)
		{
			if (!Grid.IsValidCell(num) || Grid.Solid[num])
			{
				this.obstructedTile = num;
				return false;
			}
			num = Grid.CellAbove(num);
		}
		return true;
	}

	public string GetObstruction()
	{
		if (this.obstructedTile == -1)
		{
			return null;
		}
		if (Grid.Objects[this.obstructedTile, 1] != null)
		{
			return Grid.Objects[this.obstructedTile, 1].GetComponent<Building>().Def.Name;
		}
		return string.Format(BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, Grid.Element[this.obstructedTile].tag.ProperName());
	}

	private GameObject module;

	private int bufferWidth;

	private bool hasClearSky;

	private int obstructedTile = -1;
}
