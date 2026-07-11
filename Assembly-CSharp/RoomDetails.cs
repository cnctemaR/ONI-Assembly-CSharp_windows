using System;
using STRINGS;

public class RoomDetails
{
	public static string RoomDetailString(Room room)
	{
		string text = string.Empty;
		text = text + "<b>" + ROOMS.DETAILS.HEADER + "</b>";
		RoomType roomType = room.roomType;
		foreach (RoomDetails.Detail detail in roomType.display_details)
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
		return string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
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
		string text = string.Empty;
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
						text += ((!(text == string.Empty)) ? ("\n<color=#BCBCBC>    • " + kprefabID.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED) : ("<color=#BCBCBC>    • " + kprefabID.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED));
						text += "</color>";
					}
					else
					{
						text += ((!(text == string.Empty)) ? ("\n    • " + kprefabID.GetProperName() + ": " + assignee.GetProperName()) : ("    • " + kprefabID.GetProperName() + ": " + assignee.GetProperName()));
					}
				}
			}
		}
		if (text == string.Empty)
		{
			text = ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
		}
		return string.Format(ROOMS.DETAILS.ASSIGNED_TO.NAME, text);
	});

	public static readonly RoomDetails.Detail SIZE = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.SIZE.NAME, room.cavity.numCells));

	public static readonly RoomDetails.Detail BUILDING_COUNT = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.BUILDING_COUNT.NAME, room.buildings.Count));

	public static readonly RoomDetails.Detail CREATURE_COUNT = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.CREATURE_COUNT.NAME, room.cavity.creatures.Count + room.cavity.eggs.Count));

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
