using System;
using System.Text;
using Klei.AI;
using STRINGS;
using TUNING;

public class DuplicantTemperatureDeltaAsEnergyAmountDisplayer : StandardAmountDisplayer
{
	public DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice timeSlice)
		: base(unitClass, timeSlice, null, GameUtil.IdentityDescriptorTense.Normal)
	{
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		stringBuilder.AppendFormat(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), this.formatter.GetFormattedValue(DUPLICANTSTATS.STANDARD.Temperature.Internal.IDEAL, GameUtil.TimeSlice.None));
		float num = ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * DUPLICANTSTATS.STANDARD.BaseStats.DEFAULT_MASS * 1000f;
		float num2 = 0f;
		float num3 = 0f;
		for (int num4 = 0; num4 != instance.deltaAttribute.Modifiers.Count; num4++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[num4];
			float value = attributeModifier.Value;
			if (attributeModifier.GetDescription() == CreatureSimTemperatureTransfer.RESULT_MODIFIER_NAME)
			{
				num2 = value * 5f;
				num3 += value * 5f;
			}
			else
			{
				num3 += value;
			}
		}
		stringBuilder.Append("\n\n");
		if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			stringBuilder.AppendFormat(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(num3, GameUtil.TimeSlice.PerCycle));
		}
		else
		{
			stringBuilder.AppendFormat(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(num3, GameUtil.TimeSlice.PerSecond));
			stringBuilder.Append("\n");
			stringBuilder.AppendFormat(UI.CHANGEPERSECOND, GameUtil.GetFormattedJoules(num3 * num, "F1", GameUtil.TimeSlice.None));
		}
		for (int num5 = 0; num5 != instance.deltaAttribute.Modifiers.Count; num5++)
		{
			AttributeModifier attributeModifier2 = instance.deltaAttribute.Modifiers[num5];
			float num6 = attributeModifier2.Value;
			string description = attributeModifier2.GetDescription();
			if (description == CreatureSimTemperatureTransfer.RESULT_MODIFIER_NAME)
			{
				num6 = num2;
			}
			stringBuilder.Append("\n");
			stringBuilder.AppendFormat(UI.MODIFIER_ITEM_TEMPLATE, description, GameUtil.GetFormattedHeatEnergyRate(num6 * num * 1f, GameUtil.HeatEnergyFormatterUnit.Automatic));
		}
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}
}
