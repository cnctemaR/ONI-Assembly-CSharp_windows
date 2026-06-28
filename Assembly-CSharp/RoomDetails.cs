using System;
using STRINGS;

public class RoomDetails
{
	public static string RoomDetailString(Room room)
	{
		string text = "";
		text = text + "<b>" + ROOMS.DETAILS.HEADER + "</b>";
		RoomTypes.RoomType roomType = RoomTypes.GetRoomType(room);
		foreach (RoomDetails.Detail detail in roomType.display_details)
		{
			text = text + "\n    • " + detail.resolve_string_function(room);
		}
		return text;
	}

	public static RoomDetails.Detail AVERAGE_TEMPERATURE = new RoomDetails.Detail(delegate(Room room)
	{
		float num = 0f;
		string text;
		if (num == 0f)
		{
			text = string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, UI.OVERLAYS.TEMPERATURE.EXTREMECOLD);
		}
		else
		{
			text = string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		}
		return text;
	});

	public static RoomDetails.Detail AVERAGE_ATMO_MASS = new RoomDetails.Detail(delegate(Room room)
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

	public static RoomDetails.Detail ASSIGNED_TO = new RoomDetails.Detail(delegate(Room room)
	{
		string text2 = "";
		foreach (BuildingComplete buildingComplete in room.GetPrimaryBuildings())
		{
			if (!(buildingComplete == null))
			{
				Assignable component = buildingComplete.GetComponent<Assignable>();
				IAssignableIdentity assignee = component.assignee;
				if (assignee == null)
				{
					text2 += ((!(text2 == "")) ? ("\n<color=#BCBCBC>    • " + buildingComplete.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED) : ("<color=#BCBCBC>    • " + buildingComplete.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED));
					text2 += "</color>";
				}
				else
				{
					text2 += ((!(text2 == "")) ? ("\n    • " + buildingComplete.GetProperName() + ": " + assignee.GetProperName()) : ("    • " + buildingComplete.GetProperName() + ": " + assignee.GetProperName()));
				}
			}
		}
		if (text2 == "")
		{
			text2 = ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
		}
		return string.Format(ROOMS.DETAILS.ASSIGNED_TO.NAME, text2);
	});

	public static RoomDetails.Detail SIZE = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.SIZE.NAME, room.cavity.numCells));

	public static RoomDetails.Detail BUILDING_COUNT = new RoomDetails.Detail((Room room) => string.Format(ROOMS.DETAILS.BUILDING_COUNT.NAME, room.buildings.Count));

	public static RoomDetails.Detail EFFECT = new RoomDetails.Detail((Room room) => RoomTypes.GetRoomType(room).effect);

	public class Detail
	{
		public Detail(Func<Room, string> resolve_string_function)
		{
			this.resolve_string_function = resolve_string_function;
		}

		public Func<Room, string> resolve_string_function;
	}
}
