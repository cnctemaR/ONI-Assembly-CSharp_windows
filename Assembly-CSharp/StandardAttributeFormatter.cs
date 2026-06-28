using System;
using Klei.AI;

public class StandardAttributeFormatter : IAttributeFormatter
{
	public StandardAttributeFormatter(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
	{
		this.unitClass = unitClass;
		this.deltaTimeSlice = deltaTimeSlice;
	}

	public string GetFormattedAttribute(AttributeInstance instance, bool tooltip = false)
	{
		return this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, "F2");
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		return this.GetFormattedValue(modifier.Value, this.deltaTimeSlice, "F2");
	}

	public virtual string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string simpleFormatString = "F2")
	{
		switch (this.unitClass)
		{
		case GameUtil.UnitClass.SimpleInteger:
			return GameUtil.GetFormattedInt(value, timeSlice);
		case GameUtil.UnitClass.Temperature:
			return GameUtil.GetFormattedTemperature(value, timeSlice, (timeSlice != GameUtil.TimeSlice.None) ? GameUtil.TemperatureInterpretation.Relative : GameUtil.TemperatureInterpretation.Absolute, true);
		case GameUtil.UnitClass.Mass:
			return GameUtil.GetFormattedMass(value, timeSlice, true, "{0:0.#}");
		case GameUtil.UnitClass.Calories:
			return GameUtil.GetFormattedCalories(value, timeSlice, true);
		case GameUtil.UnitClass.Percent:
			return GameUtil.GetFormattedPercent(value, timeSlice);
		case GameUtil.UnitClass.Distance:
			return GameUtil.GetFormattedDistance(value);
		}
		return GameUtil.GetFormattedSimple(value, timeSlice, simpleFormatString);
	}

	public GameUtil.UnitClass unitClass;

	public GameUtil.TimeSlice deltaTimeSlice;
}
