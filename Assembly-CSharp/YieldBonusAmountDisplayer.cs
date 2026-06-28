using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class YieldBonusAmountDisplayer : StandardAmountDisplayer
{
	public YieldBonusAmountDisplayer()
		: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle, new YieldBonusAttributeFormatter(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle))
	{
	}

	public override string GetValueString(Amount master, AmountInstance instance)
	{
		return string.Format(CREATURES.STATS.YIELDBONUS.AMOUNT_FMT, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, "F2"), this.formatter.GetFormattedValue(1f, GameUtil.TimeSlice.None, "F2"));
	}

	public override string GetDescription(Amount master, AmountInstance instance)
	{
		GameObject gameObject = instance.gameObject;
		float num = 0f;
		LocString locString = string.Empty;
		if (gameObject != null)
		{
			AmountInstance amountInstance = gameObject.GetAmounts().Get(Db.Get().Amounts.Maturity);
			if (amountInstance != null)
			{
				float num2 = instance.deltaAttribute.GetTotalDisplayValue() * 600f;
				float num3 = amountInstance.GetMax() - amountInstance.value;
				float value = instance.value;
				num = value + num2 * num3;
				if (num >= 0.8f)
				{
					locString = CREATURES.STATUSITEMS.HIGH_YIELD.NAME;
				}
				else if (num >= 0.4f)
				{
					locString = CREATURES.STATUSITEMS.NORMAL_YIELD.NAME;
				}
				else
				{
					locString = CREATURES.STATUSITEMS.LOW_YIELD.NAME;
				}
			}
		}
		return string.Format(CREATURES.STATS.YIELDBONUS.AMOUNT_DESC_FMT, new object[]
		{
			master.Name,
			this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None, "F1"),
			this.formatter.GetFormattedValue(1f, GameUtil.TimeSlice.None, "F1"),
			locString,
			this.formatter.GetFormattedValue(num, GameUtil.TimeSlice.None, "F1")
		});
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = this.GetTooltipDescription(master, instance);
		text += "\n\n";
		GameObject gameObject = instance.gameObject;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		LocString locString = string.Empty;
		if (gameObject != null)
		{
			AmountInstance amountInstance = gameObject.GetAmounts().Get(Db.Get().Amounts.Maturity);
			if (amountInstance != null)
			{
				float num4 = instance.deltaAttribute.GetTotalDisplayValue() * 600f;
				num = amountInstance.GetMax() - amountInstance.value;
				num2 = instance.value;
				num3 = num2 + num4 * num;
				if (num3 >= 0.8f)
				{
					locString = CREATURES.STATUSITEMS.HIGH_YIELD.NAME;
				}
				else if (num3 >= 0.4f)
				{
					locString = CREATURES.STATUSITEMS.NORMAL_YIELD.NAME;
				}
				else
				{
					locString = CREATURES.STATUSITEMS.LOW_YIELD.NAME;
				}
			}
		}
		text = text + string.Format(CREATURES.STATS.YIELDBONUS.CURRENT, this.formatter.GetFormattedValue(num2, GameUtil.TimeSlice.None, "F1"), 100f) + "\n";
		text = text + string.Format(CREATURES.STATS.YIELDBONUS.PREDICTION, locString, this.formatter.GetFormattedValue(num3, GameUtil.TimeSlice.None, "F1"), 100f) + "\n\n";
		if (instance.paused)
		{
			text = text + CREATURES.STATS.YIELDBONUS.PAUSED + "\n";
		}
		else
		{
			text = text + string.Format(CREATURES.STATS.YIELDBONUS.CYCLES_REMAINING, GameUtil.GetFormattedCycles(num * 600f, "F1")) + "\n";
			if (this.formatter.deltaTimeSlice == GameUtil.TimeSlice.PerCycle)
			{
				text += string.Format(UI.CHANGEPERCYCLE, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle, "F2"));
			}
			else
			{
				text += string.Format(UI.CHANGEPERSECOND, this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond, "F2"));
			}
			foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in instance.deltaAttribute)
			{
				text = text + "\n    • " + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, this.formatter.GetFormattedModifier(attributeModifierEntry.Modifier));
			}
		}
		return text;
	}
}
