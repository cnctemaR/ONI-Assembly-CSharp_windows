using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MilkProductionDisplayer : AsPercentAmountDisplayer, IVariableImageAmountDisplayer, IAmountDisplayer
{
	public MilkProductionDisplayer(GameUtil.TimeSlice deltaTimeSlice)
		: base(deltaTimeSlice)
	{
	}

	public MilkProductionDisplayer(GameUtil.TimeSlice deltaTimeSlice, Dictionary<Tag, string> customIconsPerElement)
		: base(deltaTimeSlice)
	{
		this.IconPerElement = customIconsPerElement;
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

	public Sprite GetIcon(Amount master, AmountInstance instance)
	{
		Element element = ElementLoader.FindElementByHash(instance.gameObject.GetSMI<MilkProductionMonitor.Instance>().def.element);
		string text;
		if (this.IconPerElement.TryGetValue(element.tag, out text))
		{
			return Assets.GetSprite(text);
		}
		return Assets.GetSprite(master.uiSprite);
	}

	public Dictionary<Tag, string> IconPerElement = new Dictionary<Tag, string>();
}
