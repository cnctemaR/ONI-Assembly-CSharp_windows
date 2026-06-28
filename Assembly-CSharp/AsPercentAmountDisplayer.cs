using System;
using Klei.AI;
using STRINGS;

public class AsPercentAmountDisplayer : IAmountDisplayer, IAttributeFormatter
{
	public AsPercentAmountDisplayer(GameUtil.TimeSlice deltaTimeSlice)
	{
		this.formatter = new StandardAttributeFormatter(GameUtil.UnitClass.Percent, deltaTimeSlice);
	}

	public string GetValueString(Amount master, AmountInstance instance)
	{
		return this.formatter.GetFormattedValue(this.ToPercent(instance.value, instance), GameUtil.TimeSlice.None);
	}

	public string GetDescription(Amount master, AmountInstance instance)
	{
		return string.Format("{0}: {1}", master.Name, this.formatter.GetFormattedValue(this.ToPercent(instance.value, instance), GameUtil.TimeSlice.None));
	}

	public virtual string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		return string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), this.formatter.GetFormattedValue(master.startingMin, GameUtil.TimeSlice.None));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += "\n\n";
		if (this.formatter.deltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(this.ToPercent(instance.deltaAttribute.GetTotalValue(), instance), this.formatter.deltaTimeSlice));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(this.ToPercent(instance.deltaAttribute.GetTotalValue(), instance), this.formatter.deltaTimeSlice));
		}
		text += "\n";
		foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
		{
			float modifierContribution = instance.deltaAttribute.GetModifierContribution(attributeModifierEntry.Modifier);
			text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, this.formatter.GetFormattedValue(this.ToPercent(modifierContribution, instance), this.formatter.deltaTimeSlice));
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

	private float ToPercent(float value, AmountInstance instance)
	{
		return 100f * value / instance.GetMax();
	}

	private StandardAttributeFormatter formatter;
}
