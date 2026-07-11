using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class EatXCalories : ColonyAchievementRequirement
	{
		public EatXCalories(int numCalories)
		{
			this.numCalories = numCalories;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloriesConsumed() / 1000f > (float)this.numCalories;
		}

		public override void Deserialize(IReader reader)
		{
			this.numCalories = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numCalories);
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CONSUME_CALORIES, GameUtil.GetFormattedCalories((!complete) ? RationTracker.Get().GetCaloriesConsumed() : ((float)this.numCalories * 1000f), GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories((float)this.numCalories * 1000f, GameUtil.TimeSlice.None, true));
		}

		private int numCalories;
	}
}
