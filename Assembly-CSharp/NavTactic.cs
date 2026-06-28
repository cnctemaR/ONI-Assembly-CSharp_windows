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

	public int[] GetCellPreferences(int root, CellOffset[] offsets, Navigator navigator)
	{
		int[] array = new int[offsets.Length];
		int[] array2 = new int[offsets.Length];
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(root, offsets[i]);
			int num2 = 0;
			num2 += this._overlapPenalty * NavigationReservations.Instance.GetOccupancyCount(num);
			num2 += this._rangePenalty * Mathf.Abs(this._preferredRange - Grid.GetCellDistance(root, num));
			num2 += this._pathCostPenalty * Mathf.Max(navigator.GetNavigationCost(num), 0);
			array[i] = num;
			array2[i] = num2;
		}
		Array.Sort<int, int>(array2, array);
		return array;
	}

	private int _overlapPenalty = 3;

	private int _preferredRange;

	private int _rangePenalty = 2;

	private int _pathCostPenalty = 1;
}
