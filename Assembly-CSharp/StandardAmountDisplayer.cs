using System;
using Klei.AI;
using STRINGS;

public class StandardAmountDisplayer : IAmountDisplayer, IAttributeFormatter
{
	public StandardAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
	{
		this.formatter = new StandardAttributeFormatter(unitClass, deltaTimeSlice);
	}

	public string GetValueString(Amount master, AmountInstance instance)
	{
		return this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None);
	}

	public string GetDescription(Amount master, AmountInstance instance)
	{
		return string.Format("{0}: {1}", master.Name, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), this.formatter.GetFormattedValue(master.startingMin, GameUtil.TimeSlice.None));
	}

	public string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += "\n\n";
		if (this.formatter.deltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedAttribute(instance.deltaAttribute));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedAttribute(instance.deltaAttribute));
		}
		text += "\n";
		foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
		{
			text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, this.formatter.GetFormattedModifier(attributeModifierEntry.Modifier));
		}
		return text;
	}

	public string GetFormattedAttribute(AttributeInstance instance)
	{
		return this.formatter.GetFormattedAttribute(instance);
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		return this.formatter.GetFormattedModifier(modifier);
	}

	private StandardAttributeFormatter formatter;
}
