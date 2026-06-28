using System;
using System.Collections.Generic;
using ProcGenGame;
using UnityEngine;

public class CavityVisualizer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
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
						Vector3 vector = Grid.CellToPos(num2);
						vector += Vector3.right / 2f + Vector3.up / 2f;
						Gizmos.DrawCube(vector, Vector3.one);
					}
				}
			}
		}
		if (this.spawnCells != null && this.drawSpawnCells)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
			foreach (int num3 in this.spawnCells)
			{
				Vector3 vector2 = Grid.CellToPos(num3);
				vector2 += Vector3.right / 2f + Vector3.up / 2f;
				Gizmos.DrawCube(vector2, Vector3.one);
			}
		}
	}

	public static CavityVisualizer Instance;

	public List<int> cavityCells = new List<int>();

	public List<int> spawnCells = new List<int>();

	public bool drawCavity = true;

	public bool drawSpawnCells = true;
}
