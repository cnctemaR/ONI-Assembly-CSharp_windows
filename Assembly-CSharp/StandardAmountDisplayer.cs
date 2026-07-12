using System;
using Klei.AI;
using STRINGS;

public class StandardAmountDisplayer : IAmountDisplayer
{
	public IAttributeFormatter Formatter
	{
		get
		{
			return this.formatter;
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

	public StandardAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice, StandardAttributeFormatter formatter = null, GameUtil.IdentityDescriptorTense tense = GameUtil.IdentityDescriptorTense.Normal)
	{
		this.tense = tense;
		if (formatter != null)
		{
			this.formatter = formatter;
			return;
		}
		this.formatter = new StandardAttributeFormatter(unitClass, deltaTimeSlice);
	}

	public virtual string GetValueString(Amount master, AmountInstance instance)
	{
		if (!master.showMax)
		{
			return this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None);
		}
		return string.Format("{0} / {1}", this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), this.formatter.GetFormattedValue(instance.GetMax(), GameUtil.TimeSlice.None));
	}

	public virtual string GetDescription(Amount master, AmountInstance instance)
	{
		return string.Format("{0}: {1}", master.Name, this.GetValueString(master, instance));
	}

	public virtual string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = "";
		if (master.description.IndexOf("{1}") > -1)
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), GameUtil.GetIdentityDescriptor(instance.gameObject, this.tense));
		}
		else
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
		}
		text += "\n\n";
		if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
		}
		else if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerSecond)
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond));
		}
		for (int num = 0; num != instance.deltaAttribute.Modifiers.Count; num++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[num];
			text = text + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), this.formatter.GetFormattedModifier(attributeModifier));
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

	public string GetFormattedValue(float value, GameUtil.TimeSlice time_slice)
	{
		return this.formatter.GetFormattedValue(value, time_slice);
	}

	protected StandardAttributeFormatter formatter;

	public GameUtil.IdentityDescriptorTense tense;
}
