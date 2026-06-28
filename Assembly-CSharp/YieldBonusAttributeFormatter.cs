using System;

public class YieldBonusAttributeFormatter : StandardAttributeFormatter
{
	public YieldBonusAttributeFormatter(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
		: base(unitClass, deltaTimeSlice)
	{
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string simpleFormatString = "F2")
	{
		return GameUtil.GetFormattedSimple(value * 100f, timeSlice, simpleFormatString);
	}
}
