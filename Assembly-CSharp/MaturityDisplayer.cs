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
		if (instance.GetDelta() != 0f)
		{
			float num = instance.GetMax() - instance.value;
			float num2 = num / instance.GetDelta();
			text += string.Format(CREATURES.STATS.MATURITY.TOOLTIP_GROWING, GameUtil.GetFormattedCycles(num2));
		}
		else
		{
			text += CREATURES.STATS.MATURITY.TOOLTIP_STALLED;
		}
		return text;
	}
}
