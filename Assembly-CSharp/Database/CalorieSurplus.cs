using System;
using System.IO;

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

		private double surplusAmount;
	}
}
