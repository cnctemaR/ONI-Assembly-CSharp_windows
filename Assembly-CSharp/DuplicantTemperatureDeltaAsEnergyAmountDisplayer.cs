using System;
using Klei.AI;
using STRINGS;

public class DuplicantTemperatureDeltaAsEnergyAmountDisplayer : StandardAmountDisplayer
{
	public DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice timeSlice)
		: base(unitClass, timeSlice, null)
	{
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = master.Name;
		text = text + UI.HORIZONTAL_BR_RULE + string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, null), this.formatter.GetFormattedValue(310.15f, GameUtil.TimeSlice.None, null));
		float num = ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * 30f * 1000f;
		text += "\n\n";
		if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, null));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, null));
			text = text + "\n" + string.Format(UI.CHANGEPERSECOND, GameUtil.GetFormattedJoules(instance.deltaAttribute.GetTotalDisplayValue() * num, "F1", GameUtil.TimeSlice.None));
		}
		foreach (AttributeModifier attributeModifier in instance.deltaAttribute.Modifiers)
		{
			text = text + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), GameUtil.GetFormattedHeatEnergyRate(attributeModifier.Value * num * 1f, GameUtil.HeatEnergyFormatterUnit.Automatic));
		}
		return text;
	}
}
