using System;
using Klei.AI;

public class CaloriesDisplayer : StandardAmountDisplayer
{
	public CaloriesDisplayer()
		: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal)
	{
		this.formatter = new CaloriesDisplayer.CaloriesAttributeFormatter();
	}

	public class CaloriesAttributeFormatter : StandardAttributeFormatter
	{
		public CaloriesAttributeFormatter()
			: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle)
		{
		}

		public override string GetFormattedModifier(AttributeModifier modifier)
		{
			if (modifier.IsMultiplier)
			{
				return GameUtil.GetFormattedPercent(-modifier.Value * 100f, GameUtil.TimeSlice.None);
			}
			return base.GetFormattedModifier(modifier);
		}
	}
}
