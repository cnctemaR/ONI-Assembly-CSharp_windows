using System;
using System.Collections.Generic;
using UnityEngine;

public static class LightGridManager
{
	private static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		LightGridManager.previewLux = new int[Grid.CellCount];
	}

	public static void Shutdown()
	{
		LightGridManager.previewLux = null;
		LightGridManager.previewLightCells.Clear();
	}

	public static void SetActiveWindow(Vector2I arStart, Vector2I arEnd)
	{
	}

	public static void DestroyPreview()
	{
		foreach (Tuple<int, int> tuple in LightGridManager.previewLightCells)
		{
			LightGridManager.previewLux[tuple.first] = 0;
		}
		LightGridManager.previewLightCells.Clear();
	}

	public static void CreatePreview(int origin_cell, float radius, LightShape shape, int lux)
	{
		LightGridManager.previewLightCells.Clear();
		ListPool<int, LightGridManager.LightGridEmitter>.PooledList pooledList = ListPool<int, LightGridManager.LightGridEmitter>.Allocate();
		pooledList.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, pooledList, (int)radius, shape);
		foreach (int num in pooledList)
		{
			if (Grid.IsValidCell(num))
			{
				int num2 = lux / LightGridManager.CalculateFalloff(0.5f, num, origin_cell);
				LightGridManager.previewLightCells.Add(new Tuple<int, int>(num, num2));
				LightGridManager.previewLux[num] = num2;
			}
		}
		pooledList.Recycle();
	}

	public static List<Tuple<int, int>> previewLightCells = new List<Tuple<int, int>>();

	public static int[] previewLux;

	public class LightGridEmitter
	{
		public LightGridEmitter(int cell, List<int> lit_cells, int intensity, float radius, Color colour, LightShape shape, float falloffRate = 0.5f)
		{
			this.cell = cell;
			this.radius = radius;
			this.intensity = intensity;
			this.colour = colour;
			this.shape = shape;
			this.litCells = lit_cells;
			this.falloffRate = falloffRate;
		}

		public void Add()
		{
			this.Remove();
			DiscreteShadowCaster.GetVisibleCells(this.cell, this.litCells, (int)this.radius, this.shape);
			for (int i = 0; i < this.litCells.Count; i++)
			{
				int num = this.litCells[i];
				int num2 = Mathf.Max(1, Mathf.RoundToInt(this.falloffRate * (float)Mathf.Max(Grid.GetCellDistance(num, this.cell), 1)));
				int num3 = Mathf.Max(0, Grid.LightCount[num] + this.intensity / num2);
				Grid.LightCount[num] = num3;
				LightGridManager.previewLux[num] = num3;
			}
		}

		public void Remove()
		{
			for (int i = 0; i < this.litCells.Count; i++)
			{
				int num = this.litCells[i];
				int num2 = LightGridManager.CalculateFalloff(this.falloffRate, num, this.cell);
				Grid.LightCount[num] = Mathf.Max(0, Grid.LightCount[num] - this.intensity / num2);
				LightGridManager.previewLux[num] = 0;
			}
			this.litCells.Clear();
		}

		public int cell = -1;

		public LightShape shape;

		public float radius = 4f;

		public int intensity = 1;

		public Color colour = Color.white;

		public float falloffRate = 0.5f;

		private List<int> litCells;
	}
}
