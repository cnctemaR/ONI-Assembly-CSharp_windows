using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class StandardAttributeFormatter : IAttributeFormatter
{
	public StandardAttributeFormatter(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
	{
		this.unitClass = unitClass;
		this.DeltaTimeSlice = deltaTimeSlice;
	}

	public GameUtil.TimeSlice DeltaTimeSlice { get; set; }

	public virtual string GetFormattedAttribute(AttributeInstance instance)
	{
		return this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, null);
	}

	public virtual string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return this.GetFormattedValue(modifier.Value, this.DeltaTimeSlice, null);
	}

	public virtual string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameObject parent_instance = null)
	{
		switch (this.unitClass)
		{
		case GameUtil.UnitClass.SimpleInteger:
			return GameUtil.GetFormattedInt(value, timeSlice);
		case GameUtil.UnitClass.Temperature:
			return GameUtil.GetFormattedTemperature(value, timeSlice, (timeSlice != GameUtil.TimeSlice.None) ? GameUtil.TemperatureInterpretation.Relative : GameUtil.TemperatureInterpretation.Absolute, true, false);
		case GameUtil.UnitClass.Mass:
			return GameUtil.GetFormattedMass(value, timeSlice, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		case GameUtil.UnitClass.Calories:
			return GameUtil.GetFormattedCalories(value, timeSlice, true);
		case GameUtil.UnitClass.Percent:
			return GameUtil.GetFormattedPercent(value, timeSlice);
		case GameUtil.UnitClass.Distance:
			return GameUtil.GetFormattedDistance(value);
		case GameUtil.UnitClass.Disease:
			return GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(value));
		}
		return GameUtil.GetFormattedSimple(value, timeSlice, null);
	}

	public virtual string GetTooltipDescription(Klei.AI.Attribute master, AttributeInstance instance)
	{
		return master.Name + UI.HORIZONTAL_BR_RULE + master.Description;
	}

	public virtual string GetTooltip(Klei.AI.Attribute master, AttributeInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += string.Format(DUPLICANTS.ATTRIBUTES.TOTAL_VALUE, this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, null));
		if (instance.GetBaseValue() != 0f)
		{
			text += string.Format(DUPLICANTS.ATTRIBUTES.BASE_VALUE, instance.GetBaseValue());
		}
		for (int num = 0; num != instance.Modifiers.Count; num++)
		{
			AttributeModifier attributeModifier = instance.Modifiers[num];
			string formattedString = attributeModifier.GetFormattedString(instance.gameObject);
			if (formattedString != null)
			{
				text += string.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifier.GetDescription(), formattedString);
			}
		}
		string text2 = string.Empty;
		AttributeConverters component = instance.gameObject.GetComponent<AttributeConverters>();
		if (component != null && master.converters.Count > 0)
		{
			foreach (AttributeConverterInstance attributeConverterInstance in component.converters)
			{
				if (attributeConverterInstance.converter.attribute == master)
				{
					string text3 = attributeConverterInstance.DescriptionFromAttribute();
					if (text3 != null)
					{
						text2 = text2 + "\n" + text3;
					}
				}
			}
		}
		if (text2.Length > 0)
		{
			text = text + "\n" + text2;
		}
		return text;
	}

	public GameUtil.UnitClass unitClass;
}
