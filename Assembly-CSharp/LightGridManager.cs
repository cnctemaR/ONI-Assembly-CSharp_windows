using System;
using System.Collections.Generic;
using UnityEngine;

public static class LightGridManager
{
	public static void Initialise()
	{
	}

	public static void Shutdown()
	{
		LightGridManager.previewLightCells.Clear();
	}

	public static void SetActiveWindow(Vector2I arStart, Vector2I arEnd)
	{
	}

	public static void DestroyPreview()
	{
		LightGridManager.previewLightCells.Clear();
	}

	public static void CreatePreview(int origin_cell, float radius, LightShape shape)
	{
		LightGridManager.previewLightCells.Clear();
		LightGridManager.previewLightCells.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, LightGridManager.previewLightCells, (int)radius, shape);
	}

	public static List<int> previewLightCells = new List<int>();

	public class LightGridEmitter
	{
		public LightGridEmitter(int cell, List<int> lit_cells, int intensity, float radius, Color colour, LightShape shape)
		{
			this.cell = cell;
			this.radius = radius;
			this.intensity = intensity;
			this.colour = colour;
			this.shape = shape;
			this.litCells = lit_cells;
		}

		public void Add()
		{
			this.Remove();
			DiscreteShadowCaster.GetVisibleCells(this.cell, this.litCells, (int)this.radius, this.shape);
			if (!this.litCells.Contains(this.cell))
			{
				this.litCells.Add(this.cell);
			}
			for (int i = 0; i < this.litCells.Count; i++)
			{
				int num = this.litCells[i];
				Grid.LightCount[num] = (byte)Mathf.Max(0, (int)Grid.LightCount[num] + this.intensity);
			}
		}

		public void Remove()
		{
			for (int i = 0; i < this.litCells.Count; i++)
			{
				int num = this.litCells[i];
				Grid.LightCount[num] = (byte)Mathf.Max(0, (int)Grid.LightCount[num] - this.intensity);
			}
			this.litCells.Clear();
		}

		public int cell = -1;

		public LightShape shape;

		public float radius = 4f;

		public int intensity = 1;

		public Color colour = Color.white;

		private List<int> litCells;
	}
}
