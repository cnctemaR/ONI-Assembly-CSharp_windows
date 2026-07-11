using System;
using Klei.AI;
using STRINGS;

public class QualityOfLifeAttributeFormatter : StandardAttributeFormatter
{
	public QualityOfLifeAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		return string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.DESC_FORMAT, this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject), this.GetFormattedValue(attributeInstance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject));
	}

	public override string GetTooltip(Klei.AI.Attribute master, AttributeInstance instance)
	{
		string text = base.GetTooltip(master, instance);
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		text = text + "\n\n" + string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION, this.GetFormattedValue(attributeInstance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject));
		if (instance.GetTotalDisplayValue() >= attributeInstance.GetTotalDisplayValue())
		{
			text = text + "\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_OVER;
		}
		else
		{
			text = text + "\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_UNDER;
		}
		return text;
	}
}
