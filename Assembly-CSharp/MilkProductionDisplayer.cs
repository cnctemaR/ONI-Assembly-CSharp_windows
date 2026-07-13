using System;
using Klei.AI;
using STRINGS;

public class MilkProductionDisplayer : AsPercentAmountDisplayer
{
	public MilkProductionDisplayer(GameUtil.TimeSlice deltaTimeSlice)
		: base(deltaTimeSlice)
	{
	}

	public override string GetDescription(Amount master, AmountInstance instance)
	{
		Element element = ElementLoader.FindElementByHash(instance.gameObject.GetSMI<MilkProductionMonitor.Instance>().def.element);
		return string.Format("{0}: {1}", GameUtil.SafeStringFormat(CREATURES.STATS.MILKPRODUCTION.DISPLAYED_NAME, new object[] { element.name }), this.formatter.GetFormattedValue(base.ToPercent(instance.value, instance), GameUtil.TimeSlice.None));
	}

	public override string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		Element element = ElementLoader.FindElementByHash(instance.gameObject.GetSMI<MilkProductionMonitor.Instance>().def.element);
		return string.Format(GameUtil.SafeStringFormat(master.description, new object[] { element.name }), this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
	}
}
