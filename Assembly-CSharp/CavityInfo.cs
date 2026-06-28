using System;
using System.Collections.Generic;

public class CavityInfo
{
	public CavityInfo()
	{
		this.handle = HandleVector<int>.InvalidHandle;
		this.dirty = true;
	}

	public Room room { get; private set; }

	public void SetRoom(Room room)
	{
		this.room = room;
		if (room != null)
		{
			room.cavity = this;
		}
	}

	public void AddBuilding(KPrefabID bc)
	{
		this.buildings.Add(bc);
		this.dirty = true;
	}

	public void RemoveBuilding(KPrefabID bc)
	{
		this.buildings.Remove(bc);
		this.dirty = true;
	}

	public void ReleaseResources()
	{
		if (this.room == null)
		{
			return;
		}
		foreach (KPrefabID kprefabID in this.room.buildings)
		{
			if (!(kprefabID == null))
			{
				kprefabID.Trigger(144050788, null);
				Assignable component = kprefabID.GetComponent<Assignable>();
				if (component != null && component.assignee == this.room)
				{
					component.Unassign();
				}
			}
		}
	}

	public HandleVector<int>.Handle handle;

	public bool hasDoor;

	public bool dirty;

	public int numCells;

	public List<KPrefabID> buildings = new List<KPrefabID>();
}
