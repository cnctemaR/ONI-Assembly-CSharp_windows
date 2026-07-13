using System;
using System.Collections.Generic;
using UnityEngine;

public class CavityInfo
{
	public CavityInfo()
	{
		this.handle = HandleVector<int>.InvalidHandle;
		this.dirty = true;
	}

	public int NumCells
	{
		get
		{
			if (this.cells == null)
			{
				return 0;
			}
			return this.cells.Count;
		}
	}

	public void AddEntity(KPrefabID entity)
	{
		this.otherEntities.Add(entity);
	}

	public void AddBuilding(KPrefabID bc)
	{
		this.buildings.Add(bc);
		this.dirty = true;
	}

	public void AddPlants(KPrefabID plant)
	{
		this.plants.Add(plant);
		this.dirty = true;
	}

	public void RemoveFromCavity(KPrefabID id, List<KPrefabID> listToRemove)
	{
		int num = -1;
		for (int i = 0; i < listToRemove.Count; i++)
		{
			if (id.InstanceID == listToRemove[i].InstanceID)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			listToRemove.RemoveAt(num);
		}
	}

	public void OnEnter(object data)
	{
		foreach (KPrefabID kprefabID in this.buildings)
		{
			if (kprefabID != null)
			{
				kprefabID.Trigger(-832141045, data);
			}
		}
	}

	public Vector3 GetCenter()
	{
		return new Vector3((float)(this.minX + (this.maxX - this.minX) / 2), (float)(this.minY + (this.maxY - this.minY) / 2));
	}

	public HandleVector<int>.Handle handle;

	public bool dirty;

	public List<int> cells;

	public int maxX;

	public int maxY;

	public int minX;

	public int minY;

	public Room room;

	public List<KPrefabID> buildings = new List<KPrefabID>();

	public List<KPrefabID> plants = new List<KPrefabID>();

	public List<KPrefabID> creatures = new List<KPrefabID>();

	public List<KPrefabID> fishes = new List<KPrefabID>();

	public List<KPrefabID> otherEntities = new List<KPrefabID>();

	public List<KPrefabID> eggs = new List<KPrefabID>();

	public List<KPrefabID> fish_eggs = new List<KPrefabID>();

	public OvercrowdingMonitor.Occupancy occupancy = new OvercrowdingMonitor.Occupancy();
}
