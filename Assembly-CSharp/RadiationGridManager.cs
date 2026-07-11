using System;
using System.Collections.Generic;
using UnityEngine;

public static class RadiationGridManager
{
	public static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		RadiationGridManager.previewLux = new int[Grid.CellCount];
	}

	public static void Shutdown()
	{
		RadiationGridManager.previewLux = null;
		RadiationGridManager.previewLightCells.Clear();
	}

	public static void DestroyPreview()
	{
		foreach (global::Tuple<int, int> tuple in RadiationGridManager.previewLightCells)
		{
			RadiationGridManager.previewLux[tuple.first] = 0;
		}
		RadiationGridManager.previewLightCells.Clear();
	}

	public static void CreatePreview(int origin_cell, float radius, global::LightShape shape, int lux)
	{
		RadiationGridManager.previewLightCells.Clear();
		ListPool<int, RadiationGridEmitter>.PooledList pooledList = ListPool<int, RadiationGridEmitter>.Allocate();
		pooledList.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, pooledList, (int)radius, shape);
		foreach (int num in pooledList)
		{
			if (Grid.IsValidCell(num))
			{
				int num2 = lux / RadiationGridManager.CalculateFalloff(0.5f, num, origin_cell);
				RadiationGridManager.previewLightCells.Add(new global::Tuple<int, int>(num, num2));
				RadiationGridManager.previewLux[num] = num2;
			}
		}
		pooledList.Recycle();
	}

	public static List<global::Tuple<int, int>> previewLightCells = new List<global::Tuple<int, int>>();

	public static int[] previewLux;
}
