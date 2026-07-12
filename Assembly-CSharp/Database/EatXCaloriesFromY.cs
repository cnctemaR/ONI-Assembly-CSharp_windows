using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class EatXCaloriesFromY : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public EatXCaloriesFromY(int numCalories, List<string> fromFoodType)
		{
			this.numCalories = numCalories;
			this.fromFoodType = fromFoodType;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloiresConsumedByFood(this.fromFoodType) / 1000f > (float)this.numCalories;
		}

		public void Deserialize(IReader reader)
		{
			this.numCalories = reader.ReadInt32();
			int num = reader.ReadInt32();
			this.fromFoodType = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.fromFoodType.Add(text);
			}
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIES_FROM_MEAT, GameUtil.GetFormattedCalories(complete ? ((float)this.numCalories * 1000f) : RationTracker.Get().GetCaloiresConsumedByFood(this.fromFoodType), GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories((float)this.numCalories * 1000f, GameUtil.TimeSlice.None, true));
		}

		private int numCalories;

		private List<string> fromFoodType = new List<string>();
	}
}
