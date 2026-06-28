using System;
using System.Collections.Generic;

public class NavigationReservations : KMonoBehaviour
{
	public int GetOccupancyCount(int cell)
	{
		if (this.cellOccupancyDensity.ContainsKey(cell))
		{
			return this.cellOccupancyDensity[cell];
		}
		return 0;
	}

	public bool isReserved(int cell)
	{
		return this.GetOccupancyCount(cell) > 0;
	}

	public void AddOccupancy(int cell)
	{
		if (!this.cellOccupancyDensity.ContainsKey(cell))
		{
			this.cellOccupancyDensity.Add(cell, 1);
		}
		else
		{
			Dictionary<int, int> dictionary;
			(dictionary = this.cellOccupancyDensity)[cell] = dictionary[cell] + 1;
		}
	}

	public void RemoveOccupancy(int cell)
	{
		if (this.cellOccupancyDensity.ContainsKey(cell))
		{
			Dictionary<int, int> dictionary;
			(dictionary = this.cellOccupancyDensity)[cell] = dictionary[cell] - 1;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NavigationReservations.Instance = this;
	}

	public int GetLeastOccupiedCell(int rootCell, CellOffset[] offsets)
	{
		int num = -1;
		int num2 = 99;
		for (int i = 0; i < offsets.Length; i++)
		{
			int num3 = Grid.OffsetCell(rootCell, offsets[i]);
			int occupancyCount = this.GetOccupancyCount(num3);
			if (occupancyCount < num2)
			{
				num2 = occupancyCount;
				num = num3;
			}
		}
		return num;
	}

	public int[] SortOffsetCellsByOccupants(int rootCell, CellOffset[] offsets)
	{
		int[] array = new int[offsets.Length];
		int[] array2 = new int[offsets.Length];
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(rootCell, offsets[i]);
			array[i] = num;
			array2[i] = this.GetOccupancyCount(num);
		}
		Array.Sort<int, int>(array2, array);
		return array;
	}

	public static NavigationReservations Instance;

	public static int InvalidReservation = -1;

	private Dictionary<int, int> cellOccupancyDensity = new Dictionary<int, int>();
}
