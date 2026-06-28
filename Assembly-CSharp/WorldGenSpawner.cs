using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using ProcGen;
using ProcGenGame;
using TemplateClasses;
using UnityEngine;

public class WorldGenSpawner : KMonoBehaviour
{
	public static WorldGenSpawner Instance
	{
		get
		{
			return WorldGenSpawner.instance;
		}
	}

	public bool SpawnsRemain()
	{
		return this.spawnables.Count > 0;
	}

	public void SpawnEverything()
	{
		for (int i = 0; i < this.spawnables.Count; i++)
		{
			this.spawnables[i].TrySpawn();
		}
	}

	public void ClearSpawnersInArea(Vector2 root_position, CellOffset[] area)
	{
		for (int i = 0; i < this.spawnables.Count; i++)
		{
			if (Grid.IsCellOffsetOf(Grid.PosToCell(root_position), this.spawnables[i].cell, area))
			{
				this.spawnables[i].FreeResources();
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		WorldGenSpawner.instance = this;
		if (!this.hasPlacedTemplates)
		{
			this.DoReveal();
		}
	}

	protected override void OnSpawn()
	{
		if (!this.hasPlacedTemplates)
		{
			this.PlaceTemplates();
			this.hasPlacedTemplates = true;
		}
		if (this.spawnInfos == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnInfos.Length; i++)
		{
			this.AddSpawnable(this.spawnInfos[i]);
		}
	}

	protected override void OnForcedCleanUp()
	{
		WorldGenSpawner.instance = null;
		for (int i = 0; i < this.spawnables.Count; i++)
		{
			this.spawnables[i].FreeResources();
		}
		this.spawnables.Clear();
		this.spawnables = null;
		this.spawnInfos = null;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		List<Prefab> list = new List<Prefab>();
		for (int i = 0; i < this.spawnables.Count; i++)
		{
			WorldGenSpawner.Spawnable spawnable = this.spawnables[i];
			if (!spawnable.isSpawned)
			{
				list.Add(spawnable.spawnInfo);
			}
		}
		this.spawnInfos = list.ToArray();
	}

	private void AddSpawnable(Prefab prefab)
	{
		this.spawnables.Add(new WorldGenSpawner.Spawnable(prefab));
	}

	public void AddLegacySpawner(Tag tag, int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		this.AddSpawnable(new Prefab(tag.Name, Prefab.Type.Other, vector2I.x, vector2I.y, SimHashes.Carbon, 300f, 1f, null, 0, Orientation.Neutral, null, null, 0));
	}

	private void PlaceTemplates()
	{
		this.spawnables = new List<WorldGenSpawner.Spawnable>();
		foreach (Prefab prefab in WorldGen.SpawnData.buildings)
		{
			prefab.type = Prefab.Type.Building;
			this.AddSpawnable(prefab);
		}
		foreach (Prefab prefab2 in WorldGen.SpawnData.elementalOres)
		{
			prefab2.type = Prefab.Type.Ore;
			this.AddSpawnable(prefab2);
		}
		foreach (Prefab prefab3 in WorldGen.SpawnData.otherEntities)
		{
			prefab3.type = Prefab.Type.Other;
			this.AddSpawnable(prefab3);
		}
		foreach (Prefab prefab4 in WorldGen.SpawnData.pickupables)
		{
			prefab4.type = Prefab.Type.Pickupable;
			this.AddSpawnable(prefab4);
		}
		WorldGen.SpawnData.buildings.Clear();
		WorldGen.SpawnData.elementalOres.Clear();
		WorldGen.SpawnData.otherEntities.Clear();
		WorldGen.SpawnData.pickupables.Clear();
	}

	private void DoReveal()
	{
		Game.Instance.Reset(WorldGen.SpawnData);
		for (int i = 0; i < Grid.CellCount; i++)
		{
			Grid.Revealed[i] = false;
			Grid.Spawnable[i] = 0;
		}
		float num = float.Parse(WorldGen.Settings.defaults.data["NewBaseVisibiltyInnerRadius"] as string);
		float num2 = float.Parse(WorldGen.Settings.defaults.data["NewBaseVisibiltyRadius"] as string);
		Vector2I baseStartPos = WorldGen.SpawnData.baseStartPos;
		GridVisibility.Reveal(baseStartPos.x, baseStartPos.y, num2, num);
	}

	private static WorldGenSpawner instance;

	[Serialize]
	private Prefab[] spawnInfos;

	[Serialize]
	private bool hasPlacedTemplates;

	private List<WorldGenSpawner.Spawnable> spawnables = new List<WorldGenSpawner.Spawnable>();

	private class Spawnable
	{
		public Spawnable(Prefab spawn_info)
		{
			this.spawnInfo = spawn_info;
			this.cell = Grid.XYToCell(this.spawnInfo.location_x, this.spawnInfo.location_y);
			if (Grid.Spawnable[this.cell] > 0)
			{
				this.TrySpawn();
			}
			else
			{
				this.fogOfWarPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnReveal", this, this.cell, GameScenePartitioner.Instance.fogOfWarChangedLayer, new Action<object>(this.OnReveal));
			}
		}

		public Prefab spawnInfo { get; private set; }

		public bool isSpawned { get; private set; }

		public int cell { get; private set; }

		private void OnReveal(object data)
		{
			if (Grid.Spawnable[this.cell] > 0)
			{
				this.TrySpawn();
			}
		}

		private void OnSolidChanged(object data)
		{
			if (!Grid.Solid[this.cell])
			{
				if (this.solidChangedPartitionerEntry != null)
				{
					this.solidChangedPartitionerEntry.Release();
					this.solidChangedPartitionerEntry = null;
				}
				Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
				this.Spawn();
			}
		}

		public void FreeResources()
		{
			if (this.solidChangedPartitionerEntry != null)
			{
				this.solidChangedPartitionerEntry.Release();
				this.solidChangedPartitionerEntry = null;
				if (Game.Instance != null)
				{
					Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
				}
			}
			if (this.fogOfWarPartitionerEntry != null)
			{
				this.fogOfWarPartitionerEntry.Release();
				this.fogOfWarPartitionerEntry = null;
			}
			this.isSpawned = true;
		}

		public void TrySpawn()
		{
			if (this.isSpawned)
			{
				return;
			}
			if (this.solidChangedPartitionerEntry != null)
			{
				return;
			}
			if (this.fogOfWarPartitionerEntry != null)
			{
				this.fogOfWarPartitionerEntry.Release();
				this.fogOfWarPartitionerEntry = null;
			}
			GameObject prefab = Assets.GetPrefab(this.GetPrefabTag());
			if (prefab != null)
			{
				Pickupable component = prefab.GetComponent<Pickupable>();
				if (component != null && Grid.Solid[this.cell])
				{
					this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnSolidChanged", this, this.cell, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
					Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(this.cell);
				}
				else
				{
					this.Spawn();
				}
			}
			else
			{
				this.Spawn();
			}
		}

		private Tag GetPrefabTag()
		{
			Mob mob;
			if (WorldGen.Settings.mobs.MobLookupTable.TryGetValue(this.spawnInfo.id, out mob) && mob.prefabName != null)
			{
				return new Tag(mob.prefabName);
			}
			return new Tag(this.spawnInfo.id);
		}

		private void Spawn()
		{
			this.isSpawned = true;
			GameObject gameObject = WorldGenSpawner.Spawnable.GetSpawnableCallback(this.spawnInfo.type)(this.spawnInfo, 0);
			if (gameObject != null && gameObject)
			{
				gameObject.SetActive(true);
				gameObject.Trigger(1119167081, null);
			}
			this.FreeResources();
		}

		public static WorldGenSpawner.Spawnable.PlaceEntityFn GetSpawnableCallback(Prefab.Type type)
		{
			switch (type)
			{
			case Prefab.Type.Building:
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceBuiling);
			case Prefab.Type.Ore:
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceElementalOres);
			case Prefab.Type.Pickupable:
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlacePickupables);
			case Prefab.Type.Other:
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceOtherEntities);
			default:
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceOtherEntities);
			}
		}

		private GameScenePartitionerEntry fogOfWarPartitionerEntry;

		private GameScenePartitionerEntry solidChangedPartitionerEntry;

		public delegate GameObject PlaceEntityFn(Prefab prefab, int root_cell);
	}
}
