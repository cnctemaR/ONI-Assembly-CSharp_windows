using System;
using UnityEngine;

public class NavTactic
{
	public NavTactic(int preferredRange, int rangePenalty = 1, int overlapPenalty = 1, int pathCostPenalty = 1)
	{
		this._overlapPenalty = overlapPenalty;
		this._preferredRange = preferredRange;
		this._rangePenalty = rangePenalty;
		this._pathCostPenalty = pathCostPenalty;
	}

	public NavTactic(int preferredRange, int rangePenalty, int overlapPenalty, int pathCostPenalty, int xPenalty, int preferredX, int yPenalty, int preferredY)
	{
		this._overlapPenalty = overlapPenalty;
		this._preferredRange = preferredRange;
		this._rangePenalty = rangePenalty;
		this._pathCostPenalty = pathCostPenalty;
		this._pathXCostPenalty = xPenalty;
		this._preferredX = preferredX;
		this._pathYCostPenalty = yPenalty;
		this._preferredY = preferredY;
	}

	public int GetCellPreferences(int root, CellOffset[] offsets, Navigator navigator)
	{
		int num = NavigationReservations.InvalidReservation;
		int num2 = int.MaxValue;
		for (int i = 0; i < offsets.Length; i++)
		{
			int num3 = Grid.OffsetCell(root, offsets[i]);
			int num4 = 0;
			num4 += this._overlapPenalty * NavigationReservations.Instance.GetOccupancyCount(num3);
			num4 += this._rangePenalty * Mathf.Abs(this._preferredRange - Grid.GetCellDistance(root, num3));
			num4 += this._pathCostPenalty * Mathf.Max(navigator.GetNavigationCost(num3), 0);
			num4 += this._pathXCostPenalty * Mathf.Abs(this._preferredX - Mathf.Abs(Grid.CellColumn(root) - Grid.CellColumn(num3)));
			num4 += this._pathYCostPenalty * Mathf.Abs(this._preferredY - Mathf.Abs(Grid.CellRow(root) - Grid.CellRow(num3)));
			if (num4 < num2 && navigator.CanReach(num3))
			{
				num2 = num4;
				num = num3;
			}
		}
		return num;
	}

	private int _overlapPenalty = 3;

	private int _preferredRange;

	private int _rangePenalty = 2;

	private int _pathCostPenalty = 1;

	private int _pathXCostPenalty;

	private int _preferredX;

	private int _pathYCostPenalty;

	private int _preferredY;
}
