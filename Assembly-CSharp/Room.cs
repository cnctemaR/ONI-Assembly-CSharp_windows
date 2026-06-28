using System;
using System.Collections.Generic;

public class Room : IAssignableIdentity
{
	public ushort id { get; private set; }

	public void SetID(ushort ID)
	{
		this.id = ID;
	}

	public List<BuildingComplete> buildings
	{
		get
		{
			return this.cavity.buildings;
		}
	}

	public string GetProperName()
	{
		return RoomTypes.GetRoomType(this).name;
	}

	public List<Ownables> GetOwners()
	{
		this.current_owners.Clear();
		foreach (BuildingComplete buildingComplete in this.GetPrimaryBuildings())
		{
			if (buildingComplete != null)
			{
				Ownable component = buildingComplete.GetComponent<Ownable>();
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

	public List<BuildingComplete> GetPrimaryBuildings()
	{
		this.primary_buildings.Clear();
		RoomTypes.RoomType roomType = RoomTypes.GetRoomType(this);
		if (roomType != RoomTypes.neutral_type)
		{
			foreach (BuildingComplete buildingComplete in this.buildings)
			{
				if (buildingComplete != null)
				{
					if (RoomTypes.GetRoomType(this).primary_constraint.building_criteria(buildingComplete))
					{
						this.primary_buildings.Add(buildingComplete);
					}
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

	private List<BuildingComplete> primary_buildings = new List<BuildingComplete>();

	public List<Ownables> current_owners = new List<Ownables>();
}
