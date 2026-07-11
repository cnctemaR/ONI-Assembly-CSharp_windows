using System;
using System.Collections.Generic;
using STRINGS;

public static class RoomConstraints
{
	public static string RoomCriteriaString(Room room)
	{
		string text = string.Empty;
		RoomType roomType = room.roomType;
		if (roomType != Db.Get().RoomTypes.Neutral)
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
			RoomType[] possibleRoomTypes = Db.Get().RoomTypes.GetPossibleRoomTypes(room);
			text += ((possibleRoomTypes.Length <= 1) ? string.Empty : ("<b>" + ROOMS.CRITERIA.POSSIBLE_TYPES_HEADER + "</b>"));
			foreach (RoomType roomType2 in possibleRoomTypes)
			{
				if (roomType2 != Db.Get().RoomTypes.Neutral)
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
						roomType2.Name,
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
						foreach (RoomType roomType3 in Db.Get().RoomTypes.resources)
						{
							if (roomType3 != roomType2 && roomType3 != Db.Get().RoomTypes.Neutral)
							{
								if (Db.Get().RoomTypes.HasAmbiguousRoomType(room, roomType2, roomType3))
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

	public static RoomConstraints.Constraint CEILING_HEIGHT_4 = new RoomConstraints.Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "4"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4"), null);

	public static RoomConstraints.Constraint CEILING_HEIGHT_6 = new RoomConstraints.Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 6, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "6"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6"), null);

	public static RoomConstraints.Constraint MINIMUM_SIZE_12 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells >= 12, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "12"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12"), null);

	public static RoomConstraints.Constraint MINIMUM_SIZE_32 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells >= 32, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "32"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32"), null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_64 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells <= 64, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "64"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64"), null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_96 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells <= 96, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "96"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96"), null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_120 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.numCells <= 120, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "120"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120"), null);

	public static RoomConstraints.Constraint NO_INDUSTRIAL_MACHINERY = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID kprefabID in room.buildings)
		{
			if (kprefabID.HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION, null);

	public static RoomConstraints.Constraint NO_COTS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID kprefabID2 in room.buildings)
		{
			if (kprefabID2.HasTag(RoomConstraints.ConstraintTags.Bed) && !kprefabID2.HasTag(RoomConstraints.ConstraintTags.LuxuryBed))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_COTS.NAME, ROOMS.CRITERIA.NO_COTS.DESCRIPTION, null);

	public static RoomConstraints.Constraint NO_OUTHOUSES = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID kprefabID3 in room.buildings)
		{
			if (kprefabID3.HasTag(RoomConstraints.ConstraintTags.Toilet) && !kprefabID3.HasTag(RoomConstraints.ConstraintTags.FlushToilet))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_OUTHOUSES.NAME, ROOMS.CRITERIA.NO_OUTHOUSES.DESCRIPTION, null);

	public static RoomConstraints.Constraint LUXURY_BED_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.LuxuryBed), null, 1, ROOMS.CRITERIA.LUXURY_BED_SINGLE.NAME, ROOMS.CRITERIA.LUXURY_BED_SINGLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint BED_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Bed) && !bc.HasTag(RoomConstraints.ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.BED_SINGLE.NAME, ROOMS.CRITERIA.BED_SINGLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint BUILDING_DECOR_POSITIVE = new RoomConstraints.Constraint(delegate(KPrefabID bc)
	{
		DecorProvider component = bc.GetComponent<DecorProvider>();
		return component != null && component.baseDecor > 0f;
	}, null, 1, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION, null);

	public static RoomConstraints.Constraint DECORATIVE_ITEM = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration), null, 1, ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, null);

	public static RoomConstraints.Constraint DECORATIVE_ITEM_20 = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration) && bc.HasTag(RoomConstraints.ConstraintTags.Decor20), null, 1, string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM_N.NAME, "20"), string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM_N.DESCRIPTION, "20"), null);

	public static RoomConstraints.Constraint POWER_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.PowerStation), null, 1, ROOMS.CRITERIA.POWER_STATION.NAME, ROOMS.CRITERIA.POWER_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint FARM_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.FarmStation), null, 1, ROOMS.CRITERIA.FARM_STATION.NAME, ROOMS.CRITERIA.FARM_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint RANCH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.RanchStation), null, 1, ROOMS.CRITERIA.RANCH_STATION.NAME, ROOMS.CRITERIA.RANCH_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint REC_BUILDING = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.RecBuilding), null, 1, ROOMS.CRITERIA.REC_BUILDING.NAME, ROOMS.CRITERIA.REC_BUILDING.DESCRIPTION, null);

	public static RoomConstraints.Constraint MACHINE_SHOP = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.MachineShop), null, 1, ROOMS.CRITERIA.MACHINE_SHOP.NAME, ROOMS.CRITERIA.MACHINE_SHOP.DESCRIPTION, null);

	public static RoomConstraints.Constraint FOOD_BOX = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.FoodStorage), null, 1, ROOMS.CRITERIA.FOOD_BOX.NAME, ROOMS.CRITERIA.FOOD_BOX.DESCRIPTION, null);

	public static RoomConstraints.Constraint LIGHT = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.LightSource), null, 1, ROOMS.CRITERIA.LIGHT.NAME, ROOMS.CRITERIA.LIGHT.DESCRIPTION, null);

	public static RoomConstraints.Constraint MASSAGE_TABLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.MassageTable), null, 1, ROOMS.CRITERIA.MASSAGE_TABLE.NAME, ROOMS.CRITERIA.MASSAGE_TABLE.DESCRIPTION, null);

	public static RoomConstraints.Constraint MESS_STATION_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.MessTable), null, 1, ROOMS.CRITERIA.MESS_STATION_SINGLE.NAME, ROOMS.CRITERIA.MESS_STATION_SINGLE.DESCRIPTION, new List<RoomConstraints.Constraint> { RoomConstraints.REC_BUILDING });

	public static RoomConstraints.Constraint RESEARCH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.ResearchStation), null, 1, ROOMS.CRITERIA.RESEARCH_STATION.NAME, ROOMS.CRITERIA.RESEARCH_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint TOILET = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Toilet), null, 1, ROOMS.CRITERIA.TOILET.NAME, ROOMS.CRITERIA.TOILET.DESCRIPTION, null);

	public static RoomConstraints.Constraint FLUSH_TOILET = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.FlushToilet), null, 1, ROOMS.CRITERIA.FLUSH_TOILET.NAME, ROOMS.CRITERIA.FLUSH_TOILET.DESCRIPTION, null);

	public static RoomConstraints.Constraint WASH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.WashStation), null, 1, ROOMS.CRITERIA.WASH_STATION.NAME, ROOMS.CRITERIA.WASH_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint ADVANCED_WASH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.AdvancedWashStation), null, 1, ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME, ROOMS.CRITERIA.ADVANCED_WASH_STATION.DESCRIPTION, null);

	public static RoomConstraints.Constraint CLINIC = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.CLINIC.NAME, ROOMS.CRITERIA.CLINIC.DESCRIPTION, new List<RoomConstraints.Constraint>
	{
		RoomConstraints.TOILET,
		RoomConstraints.FLUSH_TOILET,
		RoomConstraints.MESS_STATION_SINGLE
	});

	public static class ConstraintTags
	{
		public static Tag Bed = "Bed".ToTag();

		public static Tag LuxuryBed = "LuxuryBed".ToTag();

		public static Tag Toilet = "Toilet".ToTag();

		public static Tag FlushToilet = "FlushToilet".ToTag();

		public static Tag MessTable = "MessTable".ToTag();

		public static Tag Clinic = "Clinic".ToTag();

		public static Tag FoodStorage = "FoodStorage".ToTag();

		public static Tag WashStation = "WashStation".ToTag();

		public static Tag AdvancedWashStation = "AdvancedWashStation".ToTag();

		public static Tag ResearchStation = "ResearchStation".ToTag();

		public static Tag LightSource = "LightSource".ToTag();

		public static Tag MassageTable = "MassageTable".ToTag();

		public static Tag IndustrialMachinery = "IndustrialMachinery".ToTag();

		public static Tag PowerStation = "PowerStation".ToTag();

		public static Tag FarmStation = "FarmStation".ToTag();

		public static Tag CreatureRelocator = "CreatureRelocator".ToTag();

		public static Tag CreatureFeeder = "CreatureFeeder".ToTag();

		public static Tag RanchStation = "RanchStation".ToTag();

		public static Tag RecBuilding = "RecBuilding".ToTag();

		public static Tag MachineShop = "MachineShop".ToTag();

		public static Tag Decor20 = "Decor20".ToTag();
	}

	public class Constraint
	{
		public Constraint(Func<KPrefabID, bool> building_criteria, Func<Room, bool> room_criteria, int times_required = 1, string name = "", string description = "", List<RoomConstraints.Constraint> stomp_in_conflict = null)
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
				foreach (KPrefabID kprefabID in room.buildings)
				{
					if (!(kprefabID == null))
					{
						if (this.building_criteria(kprefabID))
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

		public Func<KPrefabID, bool> building_criteria;

		public List<RoomConstraints.Constraint> stomp_in_conflict;
	}
}
