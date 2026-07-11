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
		this.AddSpawnable(new Prefab(tag.Name, Prefab.Type.Other, vector2I.x, vector2I.y, SimHashes.Carbon, -1f, 1f, null, 0, Orientation.Neutral, null, null, 0));
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
		float floatSetting = WorldGen.Settings.GetFloatSetting("NewBaseVisibiltyInnerRadius");
		float floatSetting2 = WorldGen.Settings.GetFloatSetting("NewBaseVisibiltyRadius");
		Vector2I baseStartPos = WorldGen.SpawnData.baseStartPos;
		GridVisibility.Reveal(baseStartPos.x, baseStartPos.y, floatSetting2, floatSetting);
	}

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
			int num = Grid.XYToCell(this.spawnInfo.location_x, this.spawnInfo.location_y);
			GameObject prefab = Assets.GetPrefab(spawn_info.id);
			if (prefab != null)
			{
				WorldSpawnableMonitor.Def def = prefab.GetDef<WorldSpawnableMonitor.Def>();
				if (def != null && def.adjustSpawnLocationCb != null)
				{
					num = def.adjustSpawnLocationCb(num);
				}
			}
			this.cell = num;
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
				GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
				Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
				this.Spawn();
			}
		}

		public void FreeResources()
		{
			if (this.solidChangedPartitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
				if (Game.Instance != null)
				{
					Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
				}
			}
			GameScenePartitioner.Instance.Free(ref this.fogOfWarPartitionerEntry);
			this.isSpawned = true;
		}

		public void TrySpawn()
		{
			if (this.isSpawned)
			{
				return;
			}
			if (this.solidChangedPartitionerEntry.IsValid())
			{
				return;
			}
			GameScenePartitioner.Instance.Free(ref this.fogOfWarPartitionerEntry);
			GameObject prefab = Assets.GetPrefab(this.GetPrefabTag());
			if (prefab != null)
			{
				bool flag = false;
				if (prefab.GetComponent<Pickupable>() != null)
				{
					flag = true;
				}
				else if (prefab.GetDef<BurrowMonitor.Def>() != null)
				{
					flag = true;
				}
				if (flag && Grid.Solid[this.cell])
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
			Mob mob = WorldGen.Settings.mobs.GetMob(this.spawnInfo.id);
			if (mob != null && mob.prefabName != null)
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
				return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceBuilding);
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

		private HandleVector<int>.Handle fogOfWarPartitionerEntry;

		private HandleVector<int>.Handle solidChangedPartitionerEntry;

		public delegate GameObject PlaceEntityFn(Prefab prefab, int root_cell);
	}
}
