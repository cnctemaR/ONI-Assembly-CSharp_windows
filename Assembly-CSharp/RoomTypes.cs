using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public static class RoomTypes
{
	public static RoomTypes.RoomType neutral_type
	{
		get
		{
			return RoomTypes.types[0];
		}
	}

	public static Assignables[] GetAssignees(Room room)
	{
		Assignables[] array;
		if (room == null)
		{
			array = new Assignables[0];
		}
		else
		{
			RoomTypes.RoomType roomType = RoomTypes.GetRoomType(room);
			if (roomType == RoomTypes.neutral_type)
			{
				array = new Assignables[0];
			}
			else
			{
				List<Assignables> list = new List<Assignables>();
				foreach (BuildingComplete buildingComplete in room.buildings)
				{
					if (!(buildingComplete == null))
					{
						if (roomType.primary_constraint.building_criteria(buildingComplete))
						{
							Assignable component = buildingComplete.GetComponent<Assignable>();
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
				array = list.ToArray();
			}
		}
		return array;
	}

	public static RoomTypes.RoomType GetRoomType(Room room)
	{
		foreach (RoomTypes.RoomType roomType in RoomTypes.types)
		{
			if (roomType != RoomTypes.neutral_type && roomType.isSatisfactory(room) == RoomTypes.RoomType.RoomIdentificationResult.all_satisfied)
			{
				bool flag = false;
				foreach (RoomTypes.RoomType roomType2 in RoomTypes.types)
				{
					if (roomType != roomType2 && roomType2 != RoomTypes.neutral_type)
					{
						if (RoomTypes.HasAmbiguousRoomType(room, roomType, roomType2))
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
		return RoomTypes.neutral_type;
	}

	public static bool HasAmbiguousRoomType(Room room, RoomTypes.RoomType suspected_type, RoomTypes.RoomType potential_type)
	{
		RoomTypes.RoomType.RoomIdentificationResult roomIdentificationResult = potential_type.isSatisfactory(room);
		RoomTypes.RoomType.RoomIdentificationResult roomIdentificationResult2 = suspected_type.isSatisfactory(room);
		if (roomIdentificationResult == RoomTypes.RoomType.RoomIdentificationResult.all_satisfied && roomIdentificationResult2 == RoomTypes.RoomType.RoomIdentificationResult.all_satisfied)
		{
			if (potential_type.priority > suspected_type.priority)
			{
				return true;
			}
		}
		if (roomIdentificationResult != RoomTypes.RoomType.RoomIdentificationResult.primary_unsatisfied)
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
			suspected_type = RoomTypes.neutral_type;
		}
		return false;
	}

	public static RoomTypes.RoomType[] GetPossibleRoomTypes(Room room)
	{
		RoomTypes.RoomType[] array = new RoomTypes.RoomType[RoomTypes.types.Length];
		int num = 0;
		foreach (RoomTypes.RoomType roomType in RoomTypes.types)
		{
			if (roomType != RoomTypes.types[0])
			{
				if (roomType.isSatisfactory(room) == RoomTypes.RoomType.RoomIdentificationResult.all_satisfied || roomType.isSatisfactory(room) == RoomTypes.RoomType.RoomIdentificationResult.primary_satisfied)
				{
					array[num] = roomType;
					num++;
				}
			}
		}
		if (num == 0)
		{
			array[num] = RoomTypes.types[0];
			num++;
		}
		Array.Resize<RoomTypes.RoomType>(ref array, num);
		return array;
	}

	public static Dictionary<string, RoomTypes.TypeCategory> TypeCategories = new Dictionary<string, RoomTypes.TypeCategory>
	{
		{
			"None",
			new RoomTypes.TypeCategory("None", "", Color.grey)
		},
		{
			"Food",
			new RoomTypes.TypeCategory("Food", "", new Color(1f, 0.8862745f, 0.5176471f))
		},
		{
			"Sleep",
			new RoomTypes.TypeCategory("Sleep", "", new Color(0.6392157f, 1f, 0.5176471f))
		},
		{
			"Bathroom",
			new RoomTypes.TypeCategory("Bathroom", "", new Color(0.5176471f, 1f, 0.95686275f))
		},
		{
			"Hospital",
			new RoomTypes.TypeCategory("Hospital", "", new Color(1f, 0.5176471f, 0.5568628f))
		}
	};

	public static RoomTypes.RoomType[] types = new RoomTypes.RoomType[]
	{
		new RoomTypes.RoomType("Neutral", ROOMS.TYPES.NEUTRAL.NAME, ROOMS.TYPES.NEUTRAL.TOOLTIP, ROOMS.TYPES.NEUTRAL.EFFECT, 1, int.MaxValue, RoomTypes.TypeCategories["None"], null, null, new RoomDetails.Detail[]
		{
			RoomDetails.SIZE,
			RoomDetails.BUILDING_COUNT
		}, 0, false, false),
		new RoomTypes.RoomType("Latrine", ROOMS.TYPES.LATRINE.NAME, ROOMS.TYPES.LATRINE.TOOLTIP, ROOMS.TYPES.LATRINE.EFFECT, 12, 64, RoomTypes.TypeCategories["Bathroom"], RoomConstraints.TOILET, new RoomConstraints.Constraint[]
		{
			RoomConstraints.WASH_STATION,
			RoomConstraints.NO_INDUSTRIAL_MACHINERY,
			RoomConstraints.MINIMUM_SIZE_12,
			RoomConstraints.MAXIMUM_SIZE_64
		}, new RoomDetails.Detail[]
		{
			RoomDetails.SIZE,
			RoomDetails.BUILDING_COUNT
		}, 1, false, false),
		new RoomTypes.RoomType("Barracks", ROOMS.TYPES.BARRACKS.NAME, ROOMS.TYPES.BARRACKS.TOOLTIP, ROOMS.TYPES.BARRACKS.EFFECT, 12, 96, RoomTypes.TypeCategories["Sleep"], RoomConstraints.BED_SINGLE, new RoomConstraints.Constraint[]
		{
			RoomConstraints.NO_INDUSTRIAL_MACHINERY,
			RoomConstraints.MINIMUM_SIZE_12,
			RoomConstraints.MAXIMUM_SIZE_64
		}, new RoomDetails.Detail[]
		{
			RoomDetails.SIZE,
			RoomDetails.BUILDING_COUNT
		}, 1, false, false),
		new RoomTypes.RoomType("MessHall", ROOMS.TYPES.MESSHALL.NAME, ROOMS.TYPES.MESSHALL.TOOLTIP, ROOMS.TYPES.MESSHALL.EFFECT, 12, 96, RoomTypes.TypeCategories["Food"], RoomConstraints.MESS_STATION_SINGLE, new RoomConstraints.Constraint[]
		{
			RoomConstraints.NO_INDUSTRIAL_MACHINERY,
			RoomConstraints.MINIMUM_SIZE_12,
			RoomConstraints.MAXIMUM_SIZE_64
		}, new RoomDetails.Detail[]
		{
			RoomDetails.SIZE,
			RoomDetails.BUILDING_COUNT
		}, 1, false, false),
		new RoomTypes.RoomType("Hospital", ROOMS.TYPES.HOSPITAL.NAME, ROOMS.TYPES.HOSPITAL.TOOLTIP, ROOMS.TYPES.HOSPITAL.EFFECT, 12, 64, RoomTypes.TypeCategories["Hospital"], RoomConstraints.CLINIC, new RoomConstraints.Constraint[]
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
		}, 2, true, true)
	};

	public class TypeCategory
	{
		public TypeCategory(string id, string name, Color color)
		{
			this.id = id;
			this.name = name;
			this.color = color;
		}

		public string id { get; private set; }

		public string name { get; private set; }

		public Color color { get; private set; }
	}

	public class RoomType
	{
		public RoomType(string id, string name, string tooltip, string effect, int size_minimum, int size_maximum, RoomTypes.TypeCategory category, RoomConstraints.Constraint primary_constraint, RoomConstraints.Constraint[] additional_constraints, RoomDetails.Detail[] display_details, int priority = 0, bool single_assignee = false, bool priority_building_use = false)
		{
			this.id = id;
			this.name = name;
			this.tooltip = tooltip;
			this.effect = effect;
			this.category = category;
			this.primary_constraint = primary_constraint;
			this.additional_constraints = additional_constraints;
			this.display_details = display_details;
			this.priority = priority;
			this.single_assignee = single_assignee;
			this.priority_building_use = priority_building_use;
		}

		public string id { get; private set; }

		public string name { get; private set; }

		public string tooltip { get; private set; }

		public string effect { get; private set; }

		public RoomConstraints.Constraint primary_constraint { get; private set; }

		public RoomConstraints.Constraint[] additional_constraints { get; private set; }

		public int priority { get; private set; }

		public bool single_assignee { get; private set; }

		public RoomDetails.Detail[] display_details { get; private set; }

		public bool priority_building_use { get; private set; }

		public RoomTypes.TypeCategory category { get; private set; }

		public RoomTypes.RoomType.RoomIdentificationResult isSatisfactory(Room candidate_room)
		{
			if (this.primary_constraint != null)
			{
				if (!this.primary_constraint.isSatisfied(candidate_room))
				{
					return RoomTypes.RoomType.RoomIdentificationResult.primary_unsatisfied;
				}
			}
			if (this.additional_constraints != null)
			{
				foreach (RoomConstraints.Constraint constraint in this.additional_constraints)
				{
					if (!constraint.isSatisfied(candidate_room))
					{
						return RoomTypes.RoomType.RoomIdentificationResult.primary_satisfied;
					}
				}
			}
			return RoomTypes.RoomType.RoomIdentificationResult.all_satisfied;
		}

		public string GetCriteriaString()
		{
			string text = string.Concat(new string[]
			{
				"<b>",
				this.name,
				"</b>\n",
				this.tooltip,
				UI.HORIZONTAL_BR_RULE,
				ROOMS.CRITERIA.HEADER
			});
			if (this.id == RoomTypes.neutral_type.id)
			{
				text = text + "\n    • " + ROOMS.CRITERIA.NEUTRAL_TYPE;
			}
			text += ((this.primary_constraint != null) ? ("\n    • " + this.primary_constraint.name) : "");
			if (this.additional_constraints != null)
			{
				foreach (RoomConstraints.Constraint constraint in this.additional_constraints)
				{
					text = text + "\n    • " + constraint.name;
				}
			}
			return text;
		}

		public enum RoomIdentificationResult
		{
			all_satisfied,
			primary_satisfied,
			primary_unsatisfied
		}
	}
}
