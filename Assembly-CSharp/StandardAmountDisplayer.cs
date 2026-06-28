using System;
using Klei.AI;
using STRINGS;

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

	public virtual string GetValueString(Amount master, AmountInstance instance)
	{
		return this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, "F2");
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return string.Format("{0}: {1}", master.Name, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, "F2"));
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, "F2"), this.formatter.GetFormattedValue(master.startingMin, GameUtil.TimeSlice.None, "F2"));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += "\n\n";
		if (this.formatter.deltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, "F2"));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, "F2"));
		}
		text += "\n";
		foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
		{
			text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, this.formatter.GetFormattedModifier(attributeModifierEntry.Modifier));
		}
		return text;
	}

	public string GetFormattedAttribute(AttributeInstance instance, bool tooltip = false)
	{
		return this.formatter.GetFormattedAttribute(instance, tooltip);
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		return this.formatter.GetFormattedModifier(modifier);
	}

	protected StandardAttributeFormatter formatter;
}
