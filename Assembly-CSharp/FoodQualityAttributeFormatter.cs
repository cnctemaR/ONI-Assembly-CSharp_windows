using System;
using Klei.AI;
using UnityEngine;

public class FoodQualityAttributeFormatter : IAttributeFormatter
{
	public GameUtil.TimeSlice DeltaTimeSlice { get; set; }

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject);
	}

	public string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GameUtil.GetFormattedInt(modifier.Value, GameUtil.TimeSlice.None);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance)
	{
		int num = (int)value;
		return Util.StripTextFormatting(GameUtil.GetFormattedFoodQuality(num));
	}
}
