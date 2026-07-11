using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class CalorieSurplus : ColonyAchievementRequirement
	{
		public CalorieSurplus(float surplusAmount)
		{
			this.surplusAmount = (double)surplusAmount;
		}

		public override bool Success()
		{
			return (double)(RationTracker.Get().CountRations(null, true) / 1000f) >= this.surplusAmount;
		}

		public override bool Fail()
		{
			return !this.Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.surplusAmount);
		}

		public override void Deserialize(IReader reader)
		{
			this.surplusAmount = reader.ReadDouble();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIE_SURPLUS, GameUtil.GetFormattedCalories((!complete) ? RationTracker.Get().CountRations(null, true) : ((float)this.surplusAmount), GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories((float)this.surplusAmount, GameUtil.TimeSlice.None, true));
		}

		private double surplusAmount;
	}
}
