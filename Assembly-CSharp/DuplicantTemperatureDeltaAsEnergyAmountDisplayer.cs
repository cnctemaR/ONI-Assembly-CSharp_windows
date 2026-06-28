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
		string text = this.GetTooltipDescription(master, instance);
		float num = ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * 30f * 1000f;
		text += "\n\n";
		if (this.formatter.deltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, "F2"));
		}
		else
		{
			text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, "F2"));
			text = text + "\n" + string.Format(UI.CHANGEPERSECOND, GameUtil.GetFormattedJoules(instance.deltaAttribute.GetTotalDisplayValue() * num, "F1"));
		}
		text += "\n";
		foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
		{
			text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, GameUtil.GetFormattedWattage(attributeModifierEntry.Modifier.Value * num, "F1"));
		}
		return text;
	}
}
