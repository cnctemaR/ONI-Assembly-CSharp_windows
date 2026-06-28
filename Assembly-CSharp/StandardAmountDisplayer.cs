using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class StandardAmountDisplayer : IAmountDisplayer, IAttributeFormatter
{
	public StandardAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice, StandardAttributeFormatter formatter = null)
	{
		if (formatter != null)
		{
			this.formatter = formatter;
		}
		else
		{
			this.formatter = new StandardAttributeFormatter(unitClass, deltaTimeSlice);
		}
	}

	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get
		{
			return this.formatter.DeltaTimeSlice;
		}
		set
		{
			this.formatter.DeltaTimeSlice = value;
		}
	}

	public virtual string GetValueString(Amount master, AmountInstance instance)
	{
		if (!master.showMax)
		{
			StandardAttributeFormatter standardAttributeFormatter = this.formatter;
			float value = instance.value;
			GameObject gameObject = instance.gameObject;
			return standardAttributeFormatter.GetFormattedValue(value, GameUtil.TimeSlice.None, gameObject);
		}
		return string.Format("{0} / {1}", this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null), this.formatter.GetFormattedValue(instance.GetMax(), GameUtil.TimeSlice.None, null));
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return string.Format("{0}: {1}", master.Name, this.GetValueString(master, instance));
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null), this.formatter.GetFormattedValue(master.startingMin, GameUtil.TimeSlice.None, null));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += "\n\n";
		if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, null));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, null));
		}
		text += "\n";
		foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
		{
			text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.GetDescription(), this.formatter.GetFormattedModifier(attributeModifierEntry.Modifier, instance.gameObject));
		}
		return text;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return this.formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return this.formatter.GetFormattedModifier(modifier, parent_instance);
	}

	public string GetFormattedValue(float value, GameUtil.TimeSlice time_slice, GameObject parent_instance)
	{
		return this.formatter.GetFormattedValue(value, time_slice, parent_instance);
	}

	protected StandardAttributeFormatter formatter;
}
