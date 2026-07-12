using System;
using System.Collections.Generic;
using Klei.AI;

public interface IAttributeFormatter
{
	GameUtil.TimeSlice DeltaTimeSlice { get; set; }

	string GetFormattedAttribute(AttributeInstance instance);

	string GetFormattedModifier(AttributeModifier modifier);

	string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice);

	string GetTooltip(Klei.AI.Attribute master, AttributeInstance instance);

	string GetTooltip(Klei.AI.Attribute master, List<AttributeModifier> modifiers, AttributeConverters converters);
}
