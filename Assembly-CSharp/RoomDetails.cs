using System;
using System.Collections.Generic;
using STRINGS;

public class RoomDetails
{
	public static string RoomDetailString(Room room)
	{
		string text = "";
		text = text + "<b>" + ROOMS.DETAILS.HEADER + "</b>";
		foreach (RoomDetails.Detail detail in room.roomType.display_details)
		{
			text = text + "\n    • " + detail.resolve_string_function(room);
		}
		return text;
	}

	public static readonly RoomDetails.Detail AVERAGE_TEMPERATURE = new RoomDetails.Detail(delegate(Room room)
	{
		float num = 0f;
		if (num == 0f)
		{
			return string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, UI.OVERLAYS.TEMPERATURE.EXTREMECOLD);
		}
		return string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
	});

	public static readonly RoomDetails.Detail AVERAGE_ATMO_MASS = new RoomDetails.Detail(delegate(Room room)
	{
		float num2 = 0f;
		float num3 = 0f;
		if (num3 > 0f)
		{
			num2 /= num3;
		}
		else
		{
			num2 = 0f;
		}
		return string.Format(ROOMS.DETAILS.AVERAGE_ATMO_MASS.NAME, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	});

	public static readonly RoomDetails.Detail ASSIGNED_TO = new RoomDetails.Detail(delegate(Room room)
	{
		string text = "";
		foreach (KPrefabID kprefabID in room.GetPrimaryEntities())
		{
			if (!(kprefabID == null))
			{
				Assignable component = kprefabID.GetComponent<Assignable>();
				if (!(component == null))
				{
					IAssignableIdentity assignee = component.assignee;
					if (assignee == null)
					{
						text += ((text == "") ? ("<color=#BCBCBC>    • " + kprefabID.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED) : ("\n<color=#BCBCBC>    • " + kprefabID.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED));
						text += "</color>";
					}
					else
					{
						text += ((text == "") ? ("    • " + kprefabID.GetProperName() + ": " + assignee.GetProperName()) : ("\n    • " + kprefabID.GetProperName() + ": " + assignee.GetProperName()));
					}
				}
			}
		}
		if (text == "")
		{
			text = ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
		}
		return string.Format(ROOMS.DETAILS.ASSIGNED_TO.NAME, text);
	});

	public static readonly RoomDetails.Detail ORNAMENT_COUNT = new RoomDetails.Detail(delegate(Room room)
	{
		int num4 = 0;
		foreach (KPrefabID kprefabID2 in room.buildings)
		{
			if (!(kprefabID2 == null))
			{
				OrnamentReceptacle component2 = kprefabID2.GetComponent<OrnamentReceptacle>();
				if (!(component2 == null) && component2.IsHoldingOrnament && component2.IsOperational)
				{
					num4++;
				}
			}
		}
		foreach (KPrefabID kprefabID3 in room.otherEntities)
		{
			if (!(kprefabID3 == null))
			{
				OrnamentReceptacle component3 = kprefabID3.GetComponent<OrnamentReceptacle>();
				if (!(component3 == null) && component3.IsHoldingOrnament && component3.IsOperational)
				{
					num4++;
				}
			}
		}
		return GameUtil.SafeStringFormat(ROOMS.DETAILS.ORNAMENT_COUNT.NAME, new object[] { num4 });
	});

	public static readonly RoomDetails.Detail SIZE = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.SIZE.NAME, room.cavity.NumCells));

	public static readonly RoomDetails.Detail BUILDING_COUNT = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.BUILDING_COUNT.NAME, room.buildings.Count));

	public static readonly RoomDetails.Detail CREATURE_COUNT = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.CREATURE_COUNT.NAME, room.cavity.creatures.Count + room.cavity.eggs.Count));

	public static readonly RoomDetails.Detail PLANT_COUNT = new RoomDetails.Detail(delegate(Room room)
	{
		int num5 = 0;
		using (List<KPrefabID>.Enumerator enumerator3 = room.cavity.plants.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				if (!enumerator3.Current.HasTag(GameTags.PlantBranch))
				{
					num5++;
				}
			}
		}
		return string.Format(ROOMS.DETAILS.PLANT_COUNT.NAME, num5);
	});

	public static readonly RoomDetails.Detail EFFECT = new RoomDetails.Detail((Room room) => room.roomType.effect);

	public static readonly RoomDetails.Detail EFFECTS = new RoomDetails.Detail((Room room) => room.roomType.GetRoomEffectsString());

	public class Detail
	{
		public Detail(Func<Room, string> resolve_string_function)
		{
			this.resolve_string_function = resolve_string_function;
		}

		public Func<Room, string> resolve_string_function;
	}
}
