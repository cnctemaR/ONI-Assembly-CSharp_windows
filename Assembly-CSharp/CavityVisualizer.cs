using System;
using System.Collections.Generic;
using ProcGenGame;
using UnityEngine;

public class CavityVisualizer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		global::Debug.Assert(CavityVisualizer.Instance == null);
		CavityVisualizer.Instance = this;
		base.OnPrefabInit();
		foreach (TerrainCell terrainCell in MobSpawning.NaturalCavities.Keys)
		{
			foreach (HashSet<int> hashSet in MobSpawning.NaturalCavities[terrainCell])
			{
				foreach (int num in hashSet)
				{
					this.cavityCells.Add(num);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (this.drawCavity)
		{
			Color[] array = new Color[]
			{
				Color.blue,
				Color.yellow
			};
			int num = 0;
			foreach (TerrainCell terrainCell in MobSpawning.NaturalCavities.Keys)
			{
				Gizmos.color = array[num % array.Length];
				Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.125f);
				num++;
				foreach (HashSet<int> hashSet in MobSpawning.NaturalCavities[terrainCell])
				{
					foreach (int num2 in hashSet)
					{
						Gizmos.DrawCube(Grid.CellToPos(num2) + (Vector3.right / 2f + Vector3.up / 2f), Vector3.one);
					}
				}
			}
		}
		if (this.spawnCells != null && this.drawSpawnCells)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
			foreach (int num3 in this.spawnCells)
			{
				Gizmos.DrawCube(Grid.CellToPos(num3) + (Vector3.right / 2f + Vector3.up / 2f), Vector3.one);
			}
		}
	}

	public static CavityVisualizer Instance;

	public List<int> cavityCells = new List<int>();

	public List<int> spawnCells = new List<int>();

	public bool drawCavity = true;

	public bool drawSpawnCells = true;
}
