using System;
using System.Collections.Generic;

public class Room : IAssignableIdentity
{
	public List<KPrefabID> buildings
	{
		get
		{
			return this.cavity.buildings;
		}
	}

	public string GetProperName()
	{
		return Db.Get().RoomTypes.GetRoomType(this).Name;
	}

	public List<Ownables> GetOwners()
	{
		this.current_owners.Clear();
		foreach (KPrefabID kprefabID in this.GetPrimaryEntities())
		{
			if (kprefabID != null)
			{
				Ownable component = kprefabID.GetComponent<Ownable>();
				if (component != null && component.assignee != null)
				{
					foreach (Ownables ownables in component.assignee.GetOwners())
					{
						if (!this.current_owners.Contains(ownables))
						{
							this.current_owners.Add(ownables);
						}
					}
				}
			}
		}
		return this.current_owners;
	}

	public Ownables GetSoleOwner()
	{
		return this.GetOwners()[0];
	}

	public List<KPrefabID> GetPrimaryEntities()
	{
		this.primary_buildings.Clear();
		RoomType roomType = Db.Get().RoomTypes.GetRoomType(this);
		if (roomType.primary_constraint != null)
		{
			foreach (KPrefabID kprefabID in this.buildings)
			{
				if (kprefabID != null && roomType.primary_constraint.building_criteria(kprefabID))
				{
					this.primary_buildings.Add(kprefabID);
				}
			}
		}
		return this.primary_buildings;
	}

	public void CleanUp()
	{
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
	}

	public CavityInfo cavity;

	private List<KPrefabID> primary_buildings = new List<KPrefabID>();

	public List<Ownables> current_owners = new List<Ownables>();
}
