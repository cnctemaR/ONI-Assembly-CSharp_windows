using System;
using Klei.AI;
using STRINGS;

public class MaturityDisplayer : AsPercentAmountDisplayer
{
	public MaturityDisplayer()
		: base(GameUtil.TimeSlice.PerCycle)
	{
	}

	public override string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		string text = base.GetTooltipDescription(master, instance);
		Growing component = instance.gameObject.GetComponent<Growing>();
		if (component.IsGrowing())
		{
			float num = instance.GetMax() - instance.value;
			float num2 = num / instance.GetDelta();
			if (component != null && component.IsGrowing())
			{
				text += string.Format(CREATURES.STATS.MATURITY.TOOLTIP_GROWING_CROP, GameUtil.GetFormattedCycles(num2, "F1"), GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest(), "F1"));
			}
			else
			{
				text += string.Format(CREATURES.STATS.MATURITY.TOOLTIP_GROWING, GameUtil.GetFormattedCycles(num2, "F1"));
			}
		}
		else if (component.ReachedNextHarvest())
		{
			text += CREATURES.STATS.MATURITY.TOOLTIP_GROWN;
		}
		else
		{
			text += CREATURES.STATS.MATURITY.TOOLTIP_STALLED;
		}
		return text;
	}

	public override string GetDescription(Amount master, AmountInstance instance)
	{
		Growing component = instance.gameObject.GetComponent<Growing>();
		if (component != null && component.IsGrowing())
		{
			return string.Format(CREATURES.STATS.MATURITY.AMOUNT_DESC_FMT, master.Name, this.formatter.GetFormattedValue(base.ToPercent(instance.value, instance), GameUtil.TimeSlice.None, null), GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest(), "F1"));
		}
		return base.GetDescription(master, instance);
	}
}
