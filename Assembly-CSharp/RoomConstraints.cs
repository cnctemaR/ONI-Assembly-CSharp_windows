using System;
using System.Collections.Generic;
using STRINGS;

public static class RoomConstraints
{
	public static string RoomCriteriaString(Room room)
	{
		string text = string.Empty;
		RoomTypes.RoomType roomType = RoomTypes.GetRoomType(room);
		if (roomType != RoomTypes.types[0])
		{
			text = text + "<b>" + ROOMS.CRITERIA.HEADER + "</b>";
			text = text + "\n    • " + roomType.primary_constraint.name;
			if (roomType.additional_constraints != null)
			{
				foreach (RoomConstraints.Constraint constraint in roomType.additional_constraints)
				{
					if (constraint.isSatisfied(room))
					{
						text = text + "\n    • " + constraint.name;
					}
					else
					{
						text = text + "\n<color=#F44A47FF>    • " + constraint.name + "</color>";
					}
				}
			}
		}
		else
		{
			RoomTypes.RoomType[] possibleRoomTypes = RoomTypes.GetPossibleRoomTypes(room);
			text += ((possibleRoomTypes.Length <= 1) ? string.Empty : ("<b>" + ROOMS.CRITERIA.POSSIBLE_TYPES_HEADER + "</b>"));
			foreach (RoomTypes.RoomType roomType2 in possibleRoomTypes)
			{
				if (roomType2 != RoomTypes.types[0])
				{
					if (text != string.Empty)
					{
						text += "\n";
					}
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						"<b><color=#BCBCBC>    • ",
						roomType2.name,
						"</b> (",
						roomType2.primary_constraint.name,
						")</color>"
					});
					bool flag = false;
					if (roomType2.additional_constraints != null)
					{
						foreach (RoomConstraints.Constraint constraint2 in roomType2.additional_constraints)
						{
							if (!constraint2.isSatisfied(room))
							{
								flag = true;
								if (constraint2.building_criteria != null)
								{
									text = text + "\n<color=#F44A47FF>        • " + string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.MISSING_BUILDING, constraint2.name) + "</color>";
								}
								else
								{
									text = text + "\n<color=#F44A47FF>        • " + string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.FAILED, constraint2.name) + "</color>";
								}
							}
						}
					}
					if (!flag)
					{
						bool flag2 = false;
						foreach (RoomTypes.RoomType roomType3 in RoomTypes.types)
						{
							if (roomType3 != roomType2 && roomType3 != RoomTypes.neutral_type)
							{
								if (RoomTypes.HasAmbiguousRoomType(room, roomType2, roomType3))
								{
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							text = text + "\n<color=#F44A47FF>        • " + ROOMS.CRITERIA.NO_TYPE_CONFLICTS + "</color>";
						}
					}
				}
			}
		}
		return text;
	}

	public static RoomConstraints.Constraint MINIMUM_SIZE_12 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells >= 12, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "12"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12"), null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_64 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells <= 64, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "64"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64"), null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_96 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells <= 96, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "96"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96"), null);

	public static RoomConstraints.Constraint NO_INDUSTRIAL_MACHINERY = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (BuildingComplete buildingComplete in room.buildings)
		{
			if (buildingComplete.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION, null);

	public static RoomConstraints.Constraint BED_SINGLE = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.Bed), null, 1, ROOMS.CRITERIA.BED_SINGLE.NAME, ROOMS.CRITERIA.BED_SINGLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint BED_MULTIPLE = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.Bed), null, 2, ROOMS.CRITERIA.BED_MULTIPLE.NAME, ROOMS.CRITERIA.BED_MULTIPLE.DESCRIPTION, new List<RoomConstraints.Constraint> { RoomConstraints.BED_SINGLE });

	public static RoomConstraints.Constraint BUILDING_DECOR_POSITIVE = new RoomConstraints.Constraint(delegate(BuildingComplete bc)
	{
		DecorProvider component = bc.GetComponent<DecorProvider>();
		return component != null && component.baseDecor > 0f;
	}, null, 1, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION, null);

	public static RoomConstraints.Constraint CLINIC = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.CLINIC.NAME, ROOMS.CRITERIA.CLINIC.DESCRIPTION, null);

	public static RoomConstraints.Constraint FOOD_BOX = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.FoodStorage), null, 1, ROOMS.CRITERIA.FOOD_BOX.NAME, ROOMS.CRITERIA.FOOD_BOX.DESCRIPTION, null);

	public static RoomConstraints.Constraint LIGHT = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.LightSource), null, 1, ROOMS.CRITERIA.LIGHT.NAME, ROOMS.CRITERIA.LIGHT.DESCRIPTION, null);

	public static RoomConstraints.Constraint MASSAGE_TABLE = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.MassageTable), null, 1, ROOMS.CRITERIA.MASSAGE_TABLE.NAME, ROOMS.CRITERIA.MASSAGE_TABLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint MESS_STATION_SINGLE = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.MessTable), null, 1, ROOMS.CRITERIA.MESS_STATION_SINGLE.NAME, ROOMS.CRITERIA.MESS_STATION_SINGLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint MESS_STATION_MULTIPLE = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.MessTable), null, 2, ROOMS.CRITERIA.MESS_STATION_MULTIPLE.NAME, ROOMS.CRITERIA.MESS_STATION_MULTIPLE.DESCRIPTION, new List<RoomConstraints.Constraint> { RoomConstraints.MESS_STATION_SINGLE });

	public static RoomConstraints.Constraint RESEARCH_STATION = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.ResearchStation), null, 1, ROOMS.CRITERIA.RESEARCH_STATION.NAME, ROOMS.CRITERIA.RESEARCH_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint TOILET = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.Toilet), null, 1, ROOMS.CRITERIA.TOILET.NAME, ROOMS.CRITERIA.TOILET.DESCRIPTION, null);

	public static RoomConstraints.Constraint WASH_STATION = new RoomConstraints.Constraint((BuildingComplete bc) => bc.prefabid.HasPrefabTag(RoomConstraints.ConstraintTags.WashStation), null, 1, ROOMS.CRITERIA.WASH_STATION.NAME, ROOMS.CRITERIA.WASH_STATION.DESCRIPTION, null);

	public static class ConstraintTags
	{
		public static Tag Bed = "Bed".ToTag();

		public static Tag Toilet = "Toilet".ToTag();

		public static Tag MessTable = "MessTable".ToTag();

		public static Tag Clinic = "Clinic".ToTag();

		public static Tag FoodStorage = "FoodStorage".ToTag();

		public static Tag WashStation = "WashStation".ToTag();

		public static Tag ResearchStation = "ResearchStation".ToTag();

		public static Tag LightSource = "LightSource".ToTag();

		public static Tag MassageTable = "MassageTable".ToTag();

		public static Tag IndustrialMachinery = "IndustrialMachinery".ToTag();
	}

	public class Constraint
	{
		public Constraint(Func<BuildingComplete, bool> building_criteria, Func<Room, bool> room_criteria, int times_required = 1, string name = "", string description = "", List<RoomConstraints.Constraint> stomp_in_conflict = null)
		{
			this.room_criteria = room_criteria;
			this.building_criteria = building_criteria;
			this.times_required = times_required;
			this.name = name;
			this.description = description;
			this.stomp_in_conflict = stomp_in_conflict;
		}

		public bool isSatisfied(Room room)
		{
			int num = 0;
			if (this.room_criteria != null && this.room_criteria(room))
			{
				num++;
			}
			if (this.building_criteria != null)
			{
				foreach (BuildingComplete buildingComplete in room.buildings)
				{
					if (!(buildingComplete == null))
					{
						if (this.building_criteria(buildingComplete))
						{
							num++;
						}
					}
				}
			}
			return num >= this.times_required;
		}

		public string name;

		public string description;

		public int times_required = 1;

		public Func<Room, bool> room_criteria;

		public Func<BuildingComplete, bool> building_criteria;

		public List<RoomConstraints.Constraint> stomp_in_conflict;
	}
}
