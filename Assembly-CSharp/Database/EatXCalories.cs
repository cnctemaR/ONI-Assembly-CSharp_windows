using System;
using System.IO;

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

		private int numCalories;
	}
}
