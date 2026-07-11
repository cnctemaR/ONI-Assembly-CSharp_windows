using System;
using System.IO;

namespace Database
{
	public class BeforeCycleNumber : ColonyAchievementRequirement
	{
		public BeforeCycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 <= this.cycleNumber;
		}

		public override bool Fail()
		{
			return !this.Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.cycleNumber);
		}

		public override void Deserialize(IReader reader)
		{
			this.cycleNumber = reader.ReadInt32();
		}

		private int cycleNumber;
	}
}
