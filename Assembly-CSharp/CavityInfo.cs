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

	public int numCells;

	public int maxX;

	public int maxY;

	public int minX;

	public int minY;

	public Room room;

	public List<KPrefabID> buildings = new List<KPrefabID>();

	public List<KPrefabID> plants = new List<KPrefabID>();

	public List<KPrefabID> creatures = new List<KPrefabID>();

	public List<KPrefabID> eggs = new List<KPrefabID>();
}
