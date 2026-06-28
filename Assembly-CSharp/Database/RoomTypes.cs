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
				RoomDetails.BUILDING_COUNT
			}, 0, false, false));
			this.Latrine = base.Add(new RoomType("Latrine", ROOMS.TYPES.LATRINE.NAME, ROOMS.TYPES.LATRINE.TOOLTIP, ROOMS.TYPES.LATRINE.EFFECT, Db.Get().RoomTypeCategories.Bathroom, RoomConstraints.TOILET, new RoomConstraints.Constraint[]
			{
				RoomConstraints.WASH_STATION,
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 1, false, false));
			this.Barracks = base.Add(new RoomType("Barracks", ROOMS.TYPES.BARRACKS.NAME, ROOMS.TYPES.BARRACKS.TOOLTIP, ROOMS.TYPES.BARRACKS.EFFECT, Db.Get().RoomTypeCategories.Sleep, RoomConstraints.BED_SINGLE, new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 1, false, false));
			this.MessHall = base.Add(new RoomType("MessHall", ROOMS.TYPES.MESSHALL.NAME, ROOMS.TYPES.MESSHALL.TOOLTIP, ROOMS.TYPES.MESSHALL.EFFECT, Db.Get().RoomTypeCategories.Food, RoomConstraints.MESS_STATION_SINGLE, new RoomConstraints.Constraint[]
			{
				RoomConstraints.NO_INDUSTRIAL_MACHINERY,
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_64
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 1, false, false));
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
			}, 2, true, true));
			this.PowerPlant = base.Add(new RoomType("PowerPlant", ROOMS.TYPES.POWER_PLANT.NAME, ROOMS.TYPES.POWER_PLANT.TOOLTIP, ROOMS.TYPES.POWER_PLANT.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.POWER_STATION, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, true, true));
			this.Farm = base.Add(new RoomType("Farm", ROOMS.TYPES.FARM.NAME, ROOMS.TYPES.FARM.TOOLTIP, ROOMS.TYPES.FARM.EFFECT, Db.Get().RoomTypeCategories.Agricultural, RoomConstraints.FARM_STATION, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, true, true));
			this.MachineShop = new RoomType("MachineShop", ROOMS.TYPES.MACHINE_SHOP.NAME, ROOMS.TYPES.MACHINE_SHOP.TOOLTIP, ROOMS.TYPES.MACHINE_SHOP.EFFECT, Db.Get().RoomTypeCategories.Industrial, RoomConstraints.MACHINE_SHOP, new RoomConstraints.Constraint[]
			{
				RoomConstraints.MINIMUM_SIZE_12,
				RoomConstraints.MAXIMUM_SIZE_96
			}, new RoomDetails.Detail[]
			{
				RoomDetails.SIZE,
				RoomDetails.BUILDING_COUNT
			}, 2, true, true);
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
			}, 2, true, true));
		}

		public Assignables[] GetAssignees(Room room)
		{
			if (room == null)
			{
				return new Assignables[0];
			}
			RoomType roomType = this.GetRoomType(room);
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
			foreach (RoomType roomType in this)
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
			foreach (RoomType roomType in this)
			{
				if (roomType != this.Neutral && roomType.isSatisfactory(room) == RoomType.RoomIdentificationResult.all_satisfied)
				{
					bool flag = false;
					foreach (RoomType roomType2 in this)
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
			if (roomIdentificationResult == RoomType.RoomIdentificationResult.all_satisfied && roomIdentificationResult2 == RoomType.RoomIdentificationResult.all_satisfied && potential_type.priority > suspected_type.priority)
			{
				return true;
			}
			if (roomIdentificationResult != RoomType.RoomIdentificationResult.primary_unsatisfied)
			{
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
			foreach (RoomType roomType in this)
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

		public RoomType Barracks;

		public RoomType MessHall;

		public RoomType Hospital;

		public RoomType PowerPlant;

		public RoomType Farm;

		public RoomType MachineShop;

		public RoomType RecRoom;
	}
}
