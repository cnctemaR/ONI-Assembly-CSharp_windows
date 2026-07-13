using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;

public static class RoomConstraints
{
	public static Tag AddAndReturn(this List<Tag> list, Tag tag)
	{
		list.Add(tag);
		return tag;
	}

	public static string RoomCriteriaString(Room room)
	{
		string text = "";
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
			return text;
		}
		RoomTypes.RoomTypeQueryResult[] possibleRoomTypes = Db.Get().RoomTypes.GetPossibleRoomTypes(room);
		text += ((possibleRoomTypes.Length > 1) ? ("<b>" + ROOMS.CRITERIA.POSSIBLE_TYPES_HEADER + "</b>") : "");
		foreach (RoomTypes.RoomTypeQueryResult roomTypeQueryResult in possibleRoomTypes)
		{
			RoomType type = roomTypeQueryResult.Type;
			if (type != Db.Get().RoomTypes.Neutral)
			{
				if (text != "")
				{
					text += "\n";
				}
				text = string.Concat(new string[]
				{
					text,
					"<b><color=#BCBCBC>    • ",
					type.Name,
					"</b> (",
					type.primary_constraint.conflictDescription,
					")</color>"
				});
				if (roomTypeQueryResult.SatisfactionRating == RoomType.RoomIdentificationResult.all_satisfied)
				{
					bool flag = false;
					RoomTypes.RoomTypeQueryResult[] array3 = possibleRoomTypes;
					for (int j = 0; j < array3.Length; j++)
					{
						RoomType type2 = array3[j].Type;
						if (type2 != type && type2 != Db.Get().RoomTypes.Neutral && Db.Get().RoomTypes.HasAmbiguousRoomType(room, type, type2))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						text += string.Format("\n<color=#F44A47FF>{0}{1}{2}</color>", "    ", "    • ", ROOMS.CRITERIA.NO_TYPE_CONFLICTS);
					}
				}
				else
				{
					foreach (RoomConstraints.Constraint constraint2 in type.additional_constraints)
					{
						if (!constraint2.isSatisfied(room))
						{
							string text2 = string.Empty;
							if (constraint2.building_criteria != null)
							{
								text2 = string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.MISSING_BUILDING, constraint2.name);
							}
							else
							{
								text2 = string.Format(ROOMS.CRITERIA.CRITERIA_FAILED.FAILED, constraint2.name);
							}
							text = text + "\n<color=#F44A47FF>        • " + text2 + "</color>";
						}
					}
				}
			}
		}
		return text;
	}

	private static bool CheckOrnament(KPrefabID entityOrBuilding)
	{
		if (entityOrBuilding == null)
		{
			return false;
		}
		if (!entityOrBuilding.HasTag(GameTags.OrnamentDisplayer))
		{
			return false;
		}
		OrnamentReceptacle component = entityOrBuilding.GetComponent<OrnamentReceptacle>();
		return component.Occupant != null && component.Occupant.HasTag(GameTags.Ornament) && (component.operational == null || component.operational.IsOperational);
	}

	public static RoomConstraints.Constraint CEILING_HEIGHT_4 = new RoomConstraints.Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "4"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4"), null, null);

	public static RoomConstraints.Constraint CEILING_HEIGHT_6 = new RoomConstraints.Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 6, 1, string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "6"), string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6"), null, null);

	public static RoomConstraints.Constraint MINIMUM_SIZE_12 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells >= 12, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "12"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12"), null, null);

	public static RoomConstraints.Constraint MINIMUM_SIZE_24 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells >= 24, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "24"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "24"), null, null);

	public static RoomConstraints.Constraint MINIMUM_SIZE_32 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells >= 32, 1, string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "32"), string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32"), null, null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_64 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells <= 64, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "64"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64"), null, null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_96 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells <= 96, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "96"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96"), null, null);

	public static RoomConstraints.Constraint MAXIMUM_SIZE_120 = new RoomConstraints.Constraint(null, (Room room) => room.cavity.NumCells <= 120, 1, string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "120"), string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120"), null, null);

	public static RoomConstraints.Constraint NO_INDUSTRIAL_MACHINERY = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		using (List<KPrefabID>.Enumerator enumerator = room.buildings.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
				{
					return false;
				}
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME, ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint NO_COTS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID kprefabID in room.buildings)
		{
			if (kprefabID.HasTag(RoomConstraints.ConstraintTags.BedType) && !kprefabID.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_COTS.NAME, ROOMS.CRITERIA.NO_COTS.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint NO_LUXURY_BEDS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		using (List<KPrefabID>.Enumerator enumerator3 = room.buildings.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				if (enumerator3.Current.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
				{
					return false;
				}
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_COTS.NAME, ROOMS.CRITERIA.NO_COTS.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint NO_OUTHOUSES = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		foreach (KPrefabID kprefabID2 in room.buildings)
		{
			if (kprefabID2.HasTag(RoomConstraints.ConstraintTags.ToiletType) && !kprefabID2.HasTag(RoomConstraints.ConstraintTags.FlushToiletType))
			{
				return false;
			}
		}
		return true;
	}, 1, ROOMS.CRITERIA.NO_OUTHOUSES.NAME, ROOMS.CRITERIA.NO_OUTHOUSES.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint NO_MESS_STATION = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < room.buildings.Count)
		{
			flag = room.buildings[num].HasTag(RoomConstraints.ConstraintTags.MessTable);
			num++;
		}
		return !flag;
	}, 1, ROOMS.CRITERIA.NO_MESS_STATION.NAME, ROOMS.CRITERIA.NO_MESS_STATION.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint NO_BASIC_MESS_STATIONS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		bool flag2 = false;
		int num2 = 0;
		while (!flag2 && num2 < room.buildings.Count)
		{
			flag2 = room.buildings[num2].PrefabID() == "DiningTable";
			if (flag2)
			{
				break;
			}
			num2++;
		}
		return !flag2;
	}, 1, ROOMS.CRITERIA.NO_BASIC_MESS_STATIONS.NAME, ROOMS.CRITERIA.NO_BASIC_MESS_STATIONS.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint HAS_LUXURY_BED = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType), null, 1, ROOMS.CRITERIA.HAS_LUXURY_BED.NAME, ROOMS.CRITERIA.HAS_LUXURY_BED.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint HAS_BED = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.BedType) && !bc.HasTag(RoomConstraints.ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.HAS_BED.NAME, ROOMS.CRITERIA.HAS_BED.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint SCIENCE_BUILDINGS = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.ScienceBuilding), null, 2, ROOMS.CRITERIA.SCIENCE_BUILDINGS.NAME, ROOMS.CRITERIA.SCIENCE_BUILDINGS.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint BED_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.BedType) && !bc.HasTag(RoomConstraints.ConstraintTags.Clinic), delegate(Room room)
	{
		short num3 = 0;
		int num4 = 0;
		while (num3 < 2 && num4 < room.buildings.Count)
		{
			if (room.buildings[num4].HasTag(RoomConstraints.ConstraintTags.BedType))
			{
				num3 += 1;
			}
			num4++;
		}
		return num3 == 1;
	}, 1, ROOMS.CRITERIA.BED_SINGLE.NAME, ROOMS.CRITERIA.BED_SINGLE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint LUXURY_BED_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.LuxuryBedType), delegate(Room room)
	{
		short num5 = 0;
		int num6 = 0;
		while (num5 <= 2 && num6 < room.buildings.Count)
		{
			if (room.buildings[num6].HasTag(RoomConstraints.ConstraintTags.LuxuryBedType))
			{
				num5 += 1;
			}
			num6++;
		}
		return num5 == 1;
	}, 1, ROOMS.CRITERIA.LUXURYBEDTYPE.NAME, ROOMS.CRITERIA.LUXURYBEDTYPE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint BUILDING_DECOR_POSITIVE = new RoomConstraints.Constraint(delegate(KPrefabID bc)
	{
		DecorProvider component = bc.GetComponent<DecorProvider>();
		return component != null && component.baseDecor > 0f;
	}, null, 1, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME, ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint DECORATIVE_ITEM = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration), null, 1, string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, 1), string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 1), null, null);

	public static RoomConstraints.Constraint DECORATIVE_ITEM_2 = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(GameTags.Decoration), null, 2, string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, 2), string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 2), null, null);

	public static RoomConstraints.Constraint ORNAMENTDISPLAYED = new RoomConstraints.Constraint(null, null, delegate(Room room)
	{
		for (int i = 0; i < room.buildings.Count; i++)
		{
			if (RoomConstraints.CheckOrnament(room.buildings[i]))
			{
				return true;
			}
		}
		for (int j = 0; j < room.otherEntities.Count; j++)
		{
			if (RoomConstraints.CheckOrnament(room.otherEntities[j]))
			{
				return true;
			}
		}
		return false;
	}, 1, ROOMS.CRITERIA.ORNAMENT.NAME, ROOMS.CRITERIA.ORNAMENT.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint POWER_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType), delegate(Room room)
	{
		int num7 = 0;
		bool flag3 = false;
		foreach (KPrefabID kprefabID3 in room.buildings)
		{
			flag3 = flag3 || kprefabID3.HasTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);
			num7 += (kprefabID3.HasTag(RoomConstraints.ConstraintTags.PowerBuilding) ? 1 : 0);
		}
		return flag3 && num7 >= 2;
	}, 1, ROOMS.CRITERIA.POWERPLANT.NAME, ROOMS.CRITERIA.POWERPLANT.DESCRIPTION, null, ROOMS.CRITERIA.POWERPLANT.CONFLICT_DESCRIPTION);

	public static RoomConstraints.Constraint FARM_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.FarmStationType), null, 1, ROOMS.CRITERIA.FARMSTATIONTYPE.NAME, ROOMS.CRITERIA.FARMSTATIONTYPE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint RANCH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.RanchStationType), null, 1, ROOMS.CRITERIA.RANCHSTATIONTYPE.NAME, ROOMS.CRITERIA.RANCHSTATIONTYPE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint SPICE_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.SpiceStation), null, 1, ROOMS.CRITERIA.SPICESTATION.NAME, ROOMS.CRITERIA.SPICESTATION.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint COOK_TOP = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.CookTop), null, 1, ROOMS.CRITERIA.COOKTOP.NAME, ROOMS.CRITERIA.COOKTOP.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint REFRIGERATOR = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Refrigerator), null, 1, ROOMS.CRITERIA.REFRIGERATOR.NAME, ROOMS.CRITERIA.REFRIGERATOR.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint REC_BUILDING = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.RecBuilding), null, 1, ROOMS.CRITERIA.RECBUILDING.NAME, ROOMS.CRITERIA.RECBUILDING.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint MACHINE_SHOP = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.MachineShopType), null, 1, ROOMS.CRITERIA.MACHINESHOPTYPE.NAME, ROOMS.CRITERIA.MACHINESHOPTYPE.DESCRIPTION, null, null);

	[Obsolete("The light requirement constraint in rooms has been removed. This is retained solely to avoid breaking mods")]
	public static RoomConstraints.Constraint LIGHT = new RoomConstraints.Constraint(null, null, 1, ROOMS.CRITERIA.LIGHTSOURCE.NAME, ROOMS.CRITERIA.LIGHTSOURCE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint DESTRESSING_BUILDING = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.DeStressingBuilding), null, 1, ROOMS.CRITERIA.DESTRESSINGBUILDING.NAME, ROOMS.CRITERIA.DESTRESSINGBUILDING.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint MASSAGE_TABLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.IsPrefabID(RoomConstraints.ConstraintTags.MassageTable), null, 1, ROOMS.CRITERIA.MASSAGE_TABLE.NAME, ROOMS.CRITERIA.MASSAGE_TABLE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint DINING_TABLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.DiningTableType), null, 1, ROOMS.CRITERIA.DININGTABLETYPE.NAME, ROOMS.CRITERIA.DININGTABLETYPE.DESCRIPTION, new List<RoomConstraints.Constraint>
	{
		RoomConstraints.REC_BUILDING,
		RoomConstraints.MESS_STATION_SINGLE,
		RoomConstraints.MULTI_MINION_DINING_TABLE
	}, null);

	public static RoomConstraints.Constraint MESS_STATION_SINGLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.IsPrefabID("DiningTable"), null, 1, ROOMS.CRITERIA.DININGTABLETYPE.NAME, ROOMS.CRITERIA.DININGTABLETYPE.DESCRIPTION, new List<RoomConstraints.Constraint>
	{
		RoomConstraints.REC_BUILDING,
		RoomConstraints.DINING_TABLE
	}, null);

	public static RoomConstraints.Constraint MULTI_MINION_DINING_TABLE = new RoomConstraints.Constraint((KPrefabID bc) => bc.IsPrefabID("MultiMinionDiningTable") || bc.gameObject.name == "MultiMinionDiningSeat", null, 1, ROOMS.CRITERIA.MULTI_MINION_DINING_TABLE.NAME, ROOMS.CRITERIA.MULTI_MINION_DINING_TABLE.DESCRIPTION, new List<RoomConstraints.Constraint>
	{
		RoomConstraints.REC_BUILDING,
		RoomConstraints.DINING_TABLE
	}, null);

	public static RoomConstraints.Constraint TOILET = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.ToiletType), null, 1, ROOMS.CRITERIA.TOILETTYPE.NAME, ROOMS.CRITERIA.TOILETTYPE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint BIONICUPKEEP = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.BionicUpkeepType), null, 2, ROOMS.CRITERIA.BIONICUPKEEP.NAME, ROOMS.CRITERIA.BIONICUPKEEP.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint BIONIC_LUBRICATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag("OilChanger"), null, 1, ROOMS.CRITERIA.BIONIC_LUBRICATION.NAME, ROOMS.CRITERIA.BIONIC_LUBRICATION.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint BIONIC_GUNKEMPTIER = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag("GunkEmptier"), null, 1, ROOMS.CRITERIA.BIONIC_GUNKEMPTIER.NAME, ROOMS.CRITERIA.BIONIC_GUNKEMPTIER.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint FLUSH_TOILET = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.FlushToiletType), null, 1, ROOMS.CRITERIA.FLUSHTOILETTYPE.NAME, ROOMS.CRITERIA.FLUSHTOILETTYPE.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint WASH_STATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.WashStation), null, 1, ROOMS.CRITERIA.WASHSTATION.NAME, ROOMS.CRITERIA.WASHSTATION.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint ADVANCEDWASHSTATION = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.AdvancedWashStation), null, 1, ROOMS.CRITERIA.ADVANCEDWASHSTATION.NAME, ROOMS.CRITERIA.ADVANCEDWASHSTATION.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint CLINIC = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Clinic), null, 1, ROOMS.CRITERIA.CLINIC.NAME, ROOMS.CRITERIA.CLINIC.DESCRIPTION, new List<RoomConstraints.Constraint>
	{
		RoomConstraints.TOILET,
		RoomConstraints.FLUSH_TOILET,
		RoomConstraints.MESS_STATION_SINGLE
	}, null);

	public static RoomConstraints.Constraint PARK_BUILDING = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.Park), null, 1, ROOMS.CRITERIA.PARK.NAME, ROOMS.CRITERIA.PARK.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint ORIGINALTILES = new RoomConstraints.Constraint(null, (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4, 1, "", "", null, null);

	public static RoomConstraints.Constraint IS_BACKWALLED = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		bool flag4 = true;
		int num8 = (room.cavity.maxX - room.cavity.minX + 1) / 2 + 1;
		int num9 = 0;
		while (flag4 && num9 < num8)
		{
			int num10 = room.cavity.minX + num9;
			int num11 = room.cavity.maxX - num9;
			int num12 = room.cavity.minY;
			while (flag4 && num12 <= room.cavity.maxY)
			{
				int num13 = Grid.XYToCell(num10, num12);
				int num14 = Grid.XYToCell(num11, num12);
				if (Game.Instance.roomProber.GetCavityForCell(num13) == room.cavity)
				{
					GameObject gameObject = Grid.Objects[num13, 2];
					flag4 &= gameObject != null && !gameObject.HasTag(GameTags.UnderConstruction);
				}
				if (Game.Instance.roomProber.GetCavityForCell(num14) == room.cavity)
				{
					GameObject gameObject2 = Grid.Objects[num14, 2];
					flag4 &= gameObject2 != null && !gameObject2.HasTag(GameTags.UnderConstruction);
				}
				if (!flag4)
				{
					return false;
				}
				num12++;
			}
			num9++;
		}
		return flag4;
	}, 1, ROOMS.CRITERIA.IS_BACKWALLED.NAME, ROOMS.CRITERIA.IS_BACKWALLED.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint WILDANIMAL = new RoomConstraints.Constraint(null, (Room room) => room.cavity.creatures.Count + room.cavity.eggs.Count > 0, 1, ROOMS.CRITERIA.WILDANIMAL.NAME, ROOMS.CRITERIA.WILDANIMAL.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint WILDANIMALS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		int num15 = 0;
		using (List<KPrefabID>.Enumerator enumerator6 = room.cavity.creatures.GetEnumerator())
		{
			while (enumerator6.MoveNext())
			{
				if (enumerator6.Current.HasTag(GameTags.Creatures.Wild))
				{
					num15++;
				}
			}
		}
		return num15 >= 2;
	}, 1, ROOMS.CRITERIA.WILDANIMALS.NAME, ROOMS.CRITERIA.WILDANIMALS.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint WILDPLANT = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		int num16 = 0;
		foreach (KPrefabID kprefabID4 in room.cavity.plants)
		{
			if (kprefabID4 != null && !kprefabID4.HasTag(GameTags.PlantBranch))
			{
				BasicForagePlantPlanted component2 = kprefabID4.GetComponent<BasicForagePlantPlanted>();
				ReceptacleMonitor component3 = kprefabID4.GetComponent<ReceptacleMonitor>();
				if (component3 != null && !component3.Replanted)
				{
					num16++;
				}
				else if (component2 != null)
				{
					num16++;
				}
			}
		}
		return num16 >= 2;
	}, 1, ROOMS.CRITERIA.WILDPLANT.NAME, ROOMS.CRITERIA.WILDPLANT.DESCRIPTION, null, null);

	public static RoomConstraints.Constraint WILDPLANTS = new RoomConstraints.Constraint(null, delegate(Room room)
	{
		int num17 = 0;
		foreach (KPrefabID kprefabID5 in room.cavity.plants)
		{
			if (kprefabID5 != null && !kprefabID5.HasTag(GameTags.PlantBranch))
			{
				BasicForagePlantPlanted component4 = kprefabID5.GetComponent<BasicForagePlantPlanted>();
				ReceptacleMonitor component5 = kprefabID5.GetComponent<ReceptacleMonitor>();
				if (component5 != null && !component5.Replanted)
				{
					num17++;
				}
				else if (component4 != null)
				{
					num17++;
				}
			}
		}
		return num17 >= 4;
	}, 1, ROOMS.CRITERIA.WILDPLANTS.NAME, ROOMS.CRITERIA.WILDPLANTS.DESCRIPTION, null, null);

	public static class ConstraintTags
	{
		public static Tag DecorFancy { get; internal set; }

		public static string GetRoomConstraintLabelText(Tag tag)
		{
			StringEntry stringEntry = null;
			string text = "STRINGS.ROOMS.CRITERIA." + tag.ToString().ToUpper() + ".NAME";
			if (!Strings.TryGet(new StringKey(text), out stringEntry))
			{
				return ROOMS.CRITERIA.IN_CODE_ERROR.text.Replace("{0}", text);
			}
			return stringEntry;
		}

		public static List<Tag> AllTags = new List<Tag>();

		public static Tag BedType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("BedType".ToTag());

		public static Tag LuxuryBedType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("LuxuryBedType".ToTag());

		public static Tag ToiletType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("ToiletType".ToTag());

		public static Tag BionicUpkeepType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("BionicUpkeep".ToTag());

		public static Tag FlushToiletType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("FlushToiletType".ToTag());

		public static Tag MessTable = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("MessTable".ToTag());

		public static Tag DiningTableType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("DiningTableType".ToTag());

		public static Tag Clinic = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("Clinic".ToTag());

		public static Tag WashStation = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("WashStation".ToTag());

		public static Tag AdvancedWashStation = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("AdvancedWashStation".ToTag());

		public static Tag ScienceBuilding = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("ScienceBuilding".ToTag());

		public static Tag MassageTable = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("MassageTable".ToTag());

		public static Tag DeStressingBuilding = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("DeStressingBuilding".ToTag());

		public static Tag IndustrialMachinery = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("IndustrialMachinery".ToTag());

		public static Tag GeneratorType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("GeneratorType".ToTag());

		public static Tag HeavyDutyGeneratorType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("HeavyDutyGeneratorType".ToTag());

		public static Tag LightDutyGeneratorType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("LightDutyGeneratorType".ToTag());

		public static Tag PowerBuilding = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("PowerBuilding".ToTag());

		public static Tag FarmStationType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("FarmStationType".ToTag());

		public static Tag RanchStationType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("RanchStationType".ToTag());

		public static Tag SpiceStation = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("SpiceStation".ToTag());

		public static Tag CookTop = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("CookTop".ToTag());

		public static Tag Refrigerator = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("Refrigerator".ToTag());

		public static Tag RecBuilding = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("RecBuilding".ToTag());

		public static Tag MachineShopType = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("MachineShopType".ToTag());

		public static Tag Park = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("Park".ToTag());

		public static Tag NatureReserve = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("NatureReserve".ToTag());

		public static Tag RocketInterior = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("RocketInterior".ToTag());

		public static Tag Decoration = RoomConstraints.ConstraintTags.AllTags.AddAndReturn(GameTags.Decoration);

		public static Tag Ornament = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("Ornament".ToTag());

		public static Tag WarmingStation = RoomConstraints.ConstraintTags.AllTags.AddAndReturn("WarmingStation".ToTag());

		[Obsolete("The light requirement constraint in rooms has been removed. Please update any references of RoomConstraints.LightSource to GameTags.Lightsource")]
		public static Tag LightSource = "LightSource".ToTag();
	}

	public class Constraint
	{
		public Constraint(Func<KPrefabID, bool> building_criteria, Func<Room, bool> room_criteria, int times_required = 1, string name = "", string description = "", List<RoomConstraints.Constraint> stomp_in_conflict = null, string overrideConstraintConflictName = null)
			: this(null, building_criteria, room_criteria, times_required, name, description, stomp_in_conflict, overrideConstraintConflictName)
		{
		}

		public Constraint(Func<KPrefabID, bool> creature_criteria, Func<KPrefabID, bool> building_criteria, Func<Room, bool> room_criteria, int times_required = 1, string name = "", string description = "", List<RoomConstraints.Constraint> stomp_in_conflict = null, string overrideConstraintConflictName = null)
		{
			this.creature_criteria = creature_criteria;
			this.room_criteria = room_criteria;
			this.building_criteria = building_criteria;
			this.times_required = times_required;
			this.name = name;
			this.description = description;
			this.stomp_in_conflict = stomp_in_conflict;
			this.conflictDescription = ((overrideConstraintConflictName == null) ? name : overrideConstraintConflictName);
		}

		public bool isSatisfied(Room room)
		{
			int num = 0;
			if (this.room_criteria != null && !this.room_criteria(room))
			{
				return false;
			}
			if (this.building_criteria != null)
			{
				int num2 = 0;
				while (num < this.times_required && num2 < room.buildings.Count)
				{
					KPrefabID kprefabID = room.buildings[num2];
					if (!(kprefabID == null) && this.building_criteria(kprefabID))
					{
						num++;
					}
					num2++;
				}
				int num3 = 0;
				while (num < this.times_required && num3 < room.plants.Count)
				{
					KPrefabID kprefabID2 = room.plants[num3];
					if (!(kprefabID2 == null) && this.building_criteria(kprefabID2))
					{
						num++;
					}
					num3++;
				}
				return num >= this.times_required;
			}
			Func<KPrefabID, bool> func = this.creature_criteria;
			return true;
		}

		public string name;

		public string description;

		public string conflictDescription;

		public int times_required = 1;

		public Func<Room, bool> room_criteria;

		public Func<KPrefabID, bool> building_criteria;

		public Func<KPrefabID, bool> creature_criteria;

		public List<RoomConstraints.Constraint> stomp_in_conflict;
	}
}
