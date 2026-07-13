using System;
using Klei.AI;
using STRINGS;

public class ScaleGrowthDisplayer : AsPercentAmountDisplayer
{
	public ScaleGrowthDisplayer(GameUtil.TimeSlice deltaTimeSlice)
		: base(deltaTimeSlice)
	{
	}

	public override string GetDescription(Amount master, AmountInstance instance)
	{
		Tag tag = instance.gameObject.PrefabID();
		string text = (CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME.ContainsKey(tag) ? CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME[tag] : master.Name);
		return string.Format("{0}: {1}", text, this.formatter.GetFormattedValue(base.ToPercent(instance.value, instance), GameUtil.TimeSlice.None));
	}

	public override string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		Tag tag = instance.gameObject.PrefabID();
		string text = (CREATURES.STATS.SCALEGROWTH.TOOLTIP_PREFIX.ContainsKey(tag) ? CREATURES.STATS.SCALEGROWTH.TOOLTIP_PREFIX[tag] : "");
		return string.Format(GameUtil.SafeStringFormat(master.description, new object[] { text }), this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
	}
}
