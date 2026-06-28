using System;
using Klei.AI;

public class FoodQualityAttributeFormatter : IAttributeFormatter
{
	public string GetFormattedAttribute(AttributeInstance instance, bool tooltip = false)
	{
		return this.GetFormattedValue(instance.GetTotalDisplayValue(), tooltip);
	}

	public string GetFormattedModifier(AttributeModifier modifier)
	{
		return GameUtil.GetFormattedInt(modifier.Value, GameUtil.TimeSlice.None);
	}

	public string GetFormattedValue(float value, bool tooltip)
	{
		int num = (int)value;
		return Util.StripTextFormatting(GameUtil.GetFormattedFoodQuality(num, tooltip));
	}
}
