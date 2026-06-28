using System;
using Klei.AI;
using UnityEngine;

public class ToPercentAttributeFormatter : IAttributeFormatter
{
	public ToPercentAttributeFormatter(float max, GameUtil.TimeSlice deltaTimeSlice = GameUtil.TimeSlice.None)
	{
		this.max = max;
		this.DeltaTimeSlice = deltaTimeSlice;
	}

	public GameUtil.TimeSlice DeltaTimeSlice { get; set; }

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return this.GetFormattedValue(instance.GetTotalDisplayValue(), this.DeltaTimeSlice, instance.gameObject);
	}

	public string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return this.GetFormattedValue(modifier.Value, this.DeltaTimeSlice, parent_instance);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice, GameObject parent_instance)
	{
		return GameUtil.GetFormattedPercent(value / this.max * 100f, timeSlice);
	}

	public float max = 1f;
}
