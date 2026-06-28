using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SpawnChunks();
	}

	private void SpawnChunks()
	{
		List<int> list = new List<int>();
		int num = this.diameter / 2;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num2 = Grid.PosToCell(base.transform.GetPosition() + Vector3.up * (float)i + Vector3.right * (float)j);
				int num3 = Grid.PosToCell(base.transform.GetPosition() + Vector3.down * (float)i + Vector3.left * (float)j);
				if (!Grid.Solid[num2])
				{
					list.Add(num2);
				}
				if (!Grid.Solid[num3])
				{
					list.Add(num3);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		foreach (ChunkSpawner.ElementSpawn elementSpawn in this.Spawns)
		{
			float num4 = global::UnityEngine.Random.Range(elementSpawn.totalMass_min, elementSpawn.totalMass_max);
			int num5 = global::UnityEngine.Random.Range(elementSpawn.chunks_min, elementSpawn.chunks_max + 1);
			float num6 = num4 / (float)num5;
			for (int l = 0; l < num5; l++)
			{
				int num7 = global::UnityEngine.Random.Range(0, list.Count);
				Vector3 vector = Grid.CellToPos(list[num7]);
				vector.z = -2.5f;
				ElementLoader.FindElementByHash(elementSpawn.element).substance.SpawnResource(vector, num6, ElementLoader.FindElementByHash(elementSpawn.element).defaultValues.temperature, byte.MaxValue, 0, false, false);
			}
		}
	}

	public ChunkSpawner.ElementSpawn[] Spawns;

	public int diameter = 20;

	[Serializable]
	public struct ElementSpawn
	{
		public string name;

		public SimHashes element;

		public float totalMass_min;

		public float totalMass_max;

		public int chunks_min;

		public int chunks_max;
	}
}
