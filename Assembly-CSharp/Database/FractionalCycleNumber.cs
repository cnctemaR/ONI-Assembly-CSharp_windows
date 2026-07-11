using System;
using System.IO;

namespace Database
{
	public class FractionalCycleNumber : ColonyAchievementRequirement
	{
		public FractionalCycleNumber(float fractionalCycleNumber)
		{
			this.fractionalCycleNumber = fractionalCycleNumber;
		}

		public override bool Success()
		{
			int num = (int)this.fractionalCycleNumber;
			float num2 = this.fractionalCycleNumber - (float)num;
			return (float)(GameClock.Instance.GetCycle() + 1) > this.fractionalCycleNumber || (GameClock.Instance.GetCycle() + 1 == num && GameClock.Instance.GetCurrentCycleAsPercentage() >= num2);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.fractionalCycleNumber);
		}

		public override void Deserialize(IReader reader)
		{
			this.fractionalCycleNumber = reader.ReadSingle();
		}

		private float fractionalCycleNumber;
	}
}
