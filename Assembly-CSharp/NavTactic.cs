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
}
