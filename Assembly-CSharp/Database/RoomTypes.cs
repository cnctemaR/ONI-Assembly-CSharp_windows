using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class RoomTypes : ResourceSet<RoomType>
	{
		public RoomTypes(ResourceSet parent)
			: base("RoomTypes", parent)
		{
			base.Initialize();
			this.Neutral = base.Add(new RoomType("Neutral", ROOMS.TYPES.NEUTRAL.NAME, ROOMS.TYPES.NEUTRAL.TOOLTIP, ROOMS.TYPES.NEUTRAL.EFFECT, Db.Get().RoomTypeCategories.None, null, null, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT
			}, 0, null, false, false, null));
			string text = "PlumbedBathroom";
			string text2 = ROOMS.TYPES.PLUMBEDBATHROOM.NAME;
			string text3 = ROOMS.TYPES.PLUMBEDBATHROOM.TOOLTIP;
			string text4 = ROOMS.TYPES.PLUMBEDBATHROOM.EFFECT;
			RoomTypeCategory roomTypeCategory = Db.Get().RoomTypeCategories.Bathroom;
			RoomConstraints.Constraint constraint = RoomConstraints.FLUSH_TOILET;
			RoomConstraints.Constraint[] array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.ADVANCED_WASH_STATION,
				RoomConstraints.NO_OUTHOUSES,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			RoomDetails.Detail[] array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			int num = 1;
			string[] array3 = new string[] { "RoomBathroom" };
			this.PlumbedBathroom = base.Add(new RoomType(text, text2, text3, text4, roomTypeCategory, constraint, array, array2, num, null, false, false, array3));
			text4 = "Latrine";
			text3 = ROOMS.TYPES.LATRINE.NAME;
			text2 = ROOMS.TYPES.LATRINE.TOOLTIP;
			text = ROOMS.TYPES.LATRINE.EFFECT;
			roomTypeCategory = Db.Get().RoomTypeCategories.Bathroom;
			constraint = RoomConstraints.TOILET;
			array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.WASH_STATION,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			num = 1;
			RoomType[] array4 = new RoomType[] { this.PlumbedBathroom };
			array3 = new string[] { "RoomLatrine" };
			this.Latrine = base.Add(new RoomType(text4, text3, text2, text, roomTypeCategory, constraint, array, array2, num, array4, false, false, array3));
			text = "Bedroom";
			text2 = ROOMS.TYPES.BEDROOM.NAME;
			text3 = ROOMS.TYPES.BEDROOM.TOOLTIP;
			text4 = ROOMS.TYPES.BEDROOM.EFFECT;
			roomTypeCategory = Db.Get().RoomTypeCategories.Sleep;
			constraint = RoomConstraints.LUXURY_BED_SINGLE;
			array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_COTS,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.CEILING_HEIGHT_4
			};
			array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			num = 1;
			array3 = new string[] { "RoomBedroom" };
			this.Bedroom = base.Add(new RoomType(text, text2, text3, text4, roomTypeCategory, constraint, array, array2, num, null, false, false, array3));
			text4 = "Barracks";
			text3 = ROOMS.TYPES.BARRACKS.NAME;
			text2 = ROOMS.TYPES.BARRACKS.TOOLTIP;
			text = ROOMS.TYPES.BARRACKS.EFFECT;
			roomTypeCategory = Db.Get().RoomTypeCategories.Sleep;
			constraint = RoomConstraints.BED_SINGLE;
			array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			num = 1;
			array4 = new RoomType[] { this.Bedroom };
			array3 = new string[] { "RoomBarracks" };
			this.Barracks = base.Add(new RoomType(text4, text3, text2, text, roomTypeCategory, constraint, array, array2, num, array4, false, false, array3));
			text = "GreatHall";
			text2 = ROOMS.TYPES.GREATHALL.NAME;
			text3 = ROOMS.TYPES.GREATHALL.TOOLTIP;
			text4 = ROOMS.TYPES.GREATHALL.EFFECT;
			roomTypeCategory = Db.Get().RoomTypeCategories.Food;
			constraint = RoomConstraints.MESS_STATION_SINGLE;
			array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_32,
				RoomConstraints.MAXIMUM_SIZE_120,
				RoomConstraints.DECORATIVE_ITEM_20,
				RoomConstraints.REC_BUILDING
			};
			array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			num = 1;
			array3 = new string[] { "RoomGreatHall" };
			this.GreatHall = base.Add(new RoomType(text, text2, text3, text4, roomTypeCategory, constraint, array, array2, num, null, false, false, array3));
			text4 = "MessHall";
			text3 = ROOMS.TYPES.MESSHALL.NAME;
			text2 = ROOMS.TYPES.MESSHALL.TOOLTIP;
			text = ROOMS.TYPES.MESSHALL.EFFECT;
			roomTypeCategory = Db.Get().RoomTypeCategories.Food;
			constraint = RoomConstraints.MESS_STATION_SINGLE;
			array = new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			};
			array2 = new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			};
			num = 1;
			array4 = new RoomType[] { this.GreatHall };
			array3 = new string[] { "RoomMessHall" };
			this.MessHall = base.Add(new RoomType(text4, text3, text2, text, roomTypeCategory, constraint, array, array2, num, array4, false, false, array3));
			this.MassageClinic = base.Add(new RoomType("MassageClinic", ROOMS.TYPES.MASSAGE_CLINIC.NAME, ROOMS.TYPES.MASSAGE_CLINIC.TOOLTIP, ROOMS.TYPES.MASSAGE_CLINIC.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.MASSAGE_TABLE, new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			this.Hospital = base.Add(new RoomType("Hospital", ROOMS.TYPES.HOSPITAL.NAME, ROOMS.TYPES.HOSPITAL.TOOLTIP, ROOMS.TYPES.HOSPITAL.EFFECT, Db.Get().RoomTypeCategories.Hospital, RoomConstraints.CLINIC, new RoomConstraints.Constraint[]
			{
				RoomConstraints.TOILET,
				RoomConstraints.MESS_STATION_SINGLE,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			this.PowerPlant = base.Add(new RoomType("PowerPlant", ROOMS.TYPES.POWER_PLANT.NAME, ROOMS.TYPES.POWER_PLANT.TOOLTIP, ROOMS.TYPES.POWER_PLANT.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.POWER_STATION, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			this.Farm = base.Add(new RoomType("Farm", ROOMS.TYPES.FARM.NAME, ROOMS.TYPES.FARM.TOOLTIP, ROOMS.TYPES.FARM.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.FARM_STATION, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null));
			this.CreaturePen = base.Add(new RoomType("CreaturePen", ROOMS.TYPES.CREATUREPEN.NAME, ROOMS.TYPES.CREATUREPEN.TOOLTIP, ROOMS.TYPES.CREATUREPEN.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.RANCH_STATION, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT,
				RoomDetails.CREATURE_COUNT
			}, 2, null, true, true, null));
			this.MachineShop = new RoomType("MachineShop", ROOMS.TYPES.MACHINE_SHOP.NAME, ROOMS.TYPES.MACHINE_SHOP.TOOLTIP, ROOMS.TYPES.MACHINE_SHOP.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.MACHINE_SHOP, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, null, true, true, null);
			this.RecRoom = base.Add(new RoomType("RecRoom", ROOMS.TYPES.REC_ROOM.NAME, ROOMS.TYPES.REC_ROOM.TOOLTIP, ROOMS.TYPES.REC_ROOM.EFFECT, Db.Get().RoomTypeCategories.Recreation, RoomConstraints.REC_BUILDING, new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.DECORATIVE_ITEM,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 0, null, true, true, null));
		}

		public Assignables[] GetAssignees(Room room)
		{
			if (room == null)
			{
				return new Assignables[0];
			}
			RoomType roomType = room.roomType;
			if (roomType.primary_constraint == null)
			{
				return new Assignables[0];
			}
			List<Assignables> list = new List<Assignables>();
			foreach (KPrefabID kprefabID in room.buildings)
			{
				if (!(kprefabID == null))
				{
					if (roomType.primary_constraint.building_criteria(kprefabID))
					{
						Assignable component = kprefabID.GetComponent<Assignable>();
						if (component.assignee != null)
						{
							foreach (Ownables ownables in component.assignee.GetOwners())
							{
								if (!list.Contains(ownables))
								{
									list.Add(ownables);
								}
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		public RoomType GetRoomTypeForID(string id)
		{
			foreach (RoomType roomType in this.resources)
			{
				if (roomType.Id == id)
				{
					return roomType;
				}
			}
			return null;
		}

		public RoomType GetRoomType(Room room)
		{
			foreach (RoomType roomType in this.resources)
			{
				if (roomType != this.Neutral && roomType.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied)
				{
					bool flag = false;
					foreach (RoomType roomType2 in this.resources)
					{
						if (roomType != roomType2 && roomType2 != this.Neutral)
						{
							if (this.HasAmbiguousRoomType(room, roomType, roomType2))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return roomType;
					}
				}
			}
			return this.Neutral;
		}

		public bool HasAmbiguousRoomType(Room room, RoomType suspected_type, RoomType potential_type)
		{
			RoomType.RoomIdentificationResult roomIdentificationResult = potential_type.isSatisfactory(room);
			RoomType.RoomIdentificationResult roomIdentificationResult2 = suspected_type.isSatisfactory(room);
			if (roomIdentificationResult == RoomType.RoomIdentificationResult.all_satisfied && roomIdentificationResult2 == RoomType.RoomIdentificationResult.all_satisfied)
			{
				if (potential_type.priority > suspected_type.priority)
				{
					return true;
				}
				if (suspected_type.upgrade_paths != null && Array.IndexOf<RoomType>(suspected_type.upgrade_paths, potential_type) != -1)
				{
					return true;
				}
				if (potential_type.upgrade_paths != null && Array.IndexOf<RoomType>(potential_type.upgrade_paths, suspected_type) != -1)
				{
					return false;
				}
			}
			if (roomIdentificationResult != RoomType.RoomIdentificationResult.primary_unsatisfied)
			{
				if (suspected_type.upgrade_paths != null && Array.IndexOf<RoomType>(suspected_type.upgrade_paths, potential_type) != -1)
				{
					return false;
				}
				if (suspected_type.primary_constraint != potential_type.primary_constraint)
				{
					bool flag = false;
					if (suspected_type.primary_constraint.stomp_in_conflict != null && suspected_type.primary_constraint.stomp_in_conflict.Contains(potential_type.primary_constraint))
					{
						flag = true;
					}
					else if (suspected_type.additional_constraints != null)
					{
						foreach (RoomConstraints.Constraint constraint in suspected_type.additional_constraints)
						{
							if (constraint == potential_type.primary_constraint || (constraint.stomp_in_conflict != null && constraint.stomp_in_conflict.Contains(potential_type.primary_constraint)))
							{
								flag = true;
								break;
							}
						}
					}
					return !flag;
				}
				suspected_type = this.Neutral;
			}
			return false;
		}

		public RoomType[] GetPossibleRoomTypes(Room room)
		{
			RoomType[] array = new RoomType[this.Count];
			int num = 0;
			foreach (RoomType roomType in this.resources)
			{
				if (roomType != this.Neutral)
				{
					if (roomType.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied || roomType.isSatisfactory(room) == RoomType.RoomIdentificationResult.primary_satisfied)
					{
						array[num] = roomType;
						num++;
					}
				}
			}
			if (num == 0)
			{
				array[num] = this.Neutral;
				num++;
			}
			Array.Resize<RoomType>(ref array, num);
			return array;
		}

		public RoomType Neutral;

		public RoomType Latrine;

		public RoomType PlumbedBathroom;

		public RoomType Barracks;

		public RoomType Bedroom;

		public RoomType MessHall;

		public RoomType GreatHall;

		public RoomType Hospital;

		public RoomType MassageClinic;

		public RoomType PowerPlant;

		public RoomType Farm;

		public RoomType CreaturePen;

		public RoomType MachineShop;

		public RoomType RecRoom;
	}
}
