using System;
using System.Collections.Generic;
using UnityEngine;

public class RadiationGridEmitter
{
	public RadiationGridEmitter(int cell, List<int> lit_cells, int intensity, float radius, global::LightShape shape, float falloffRate = 0.5f)
	{
		this.cell = cell;
		this.radius = radius;
		this.intensity = intensity;
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
			int num3 = Mathf.Max(0, Grid.RadiationCount[num] + this.intensity / num2);
			Grid.RadiationCount[num] = num3;
			RadiationGridManager.previewLux[num] = num3;
		}
	}

	public void Remove()
	{
		for (int i = 0; i < this.litCells.Count; i++)
		{
			int num = this.litCells[i];
			int num2 = RadiationGridManager.CalculateFalloff(this.falloffRate, num, this.cell);
			Grid.RadiationCount[num] = Mathf.Max(0, Grid.RadiationCount[num] - this.intensity / num2);
			RadiationGridManager.previewLux[num] = 0;
		}
		this.litCells.Clear();
	}

	public int cell = -1;

	public global::LightShape shape;

	public float radius = 4f;

	public int intensity = 1;

	public float falloffRate = 0.5f;

	private List<int> litCells;
}
