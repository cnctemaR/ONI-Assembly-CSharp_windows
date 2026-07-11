using System;
using System.Collections.Generic;

public class NavigationReservations : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		NavigationReservations.Instance = null;
	}

	public int GetOccupancyCount(int cell)
	{
		if (this.cellOccupancyDensity.ContainsKey(cell))
		{
			return this.cellOccupancyDensity[cell];
		}
		return 0;
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
		int num = 0;
		if (this.cellOccupancyDensity.TryGetValue(cell, out num))
		{
			if (num == 1)
			{
				this.cellOccupancyDensity.Remove(cell);
				return;
			}
			this.cellOccupancyDensity[cell] = num - 1;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NavigationReservations.Instance = this;
	}

	public static NavigationReservations Instance;

	public static int InvalidReservation = -1;

	private Dictionary<int, int> cellOccupancyDensity = new Dictionary<int, int>();
}
