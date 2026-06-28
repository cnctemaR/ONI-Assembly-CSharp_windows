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

	public void AddBuilding(BuildingComplete bc)
	{
		this.buildings.Add(bc);
		this.dirty = true;
	}

	public void RemoveBuilding(BuildingComplete bc)
	{
		this.buildings.Remove(bc);
		this.dirty = true;
	}

	public void ReleaseResources()
	{
		if (this.room != null)
		{
			foreach (BuildingComplete buildingComplete in this.room.buildings)
			{
				if (!(buildingComplete == null))
				{
					Assignable assignable = buildingComplete.assignable;
					if (assignable != null)
					{
						if (assignable.assignee == this.room)
						{
							assignable.Unassign();
							assignable.Trigger(2070884250, null);
						}
					}
				}
			}
		}
	}

	public HandleVector<int>.Handle handle;

	public bool hasDoor = false;

	public bool dirty = false;

	public int numCells;

	public List<BuildingComplete> buildings = new List<BuildingComplete>();
}
