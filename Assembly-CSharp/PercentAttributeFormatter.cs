using System;
using Klei.AI;
using UnityEngine;

public class PercentAttributeFormatter : IAttributeFormatter
{
	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get
		{
			return GameUtil.TimeSlice.None;
		}
		set
		{
		}
	}

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
		return GameUtil.GetFormattedPercent(value * 100f, timeSlice);
	}
}
