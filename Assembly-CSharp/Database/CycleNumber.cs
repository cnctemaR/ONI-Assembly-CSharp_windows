using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class CycleNumber : VictoryColonyAchievementRequirement
	{
		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE, this.cycleNumber);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE_DESCRIPTION, this.cycleNumber);
		}

		public CycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 >= this.cycleNumber;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.cycleNumber);
		}

		public override void Deserialize(IReader reader)
		{
			this.cycleNumber = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CYCLE_NUMBER, complete ? this.cycleNumber : (GameClock.Instance.GetCycle() + 1), this.cycleNumber);
		}

		private int cycleNumber;
	}
}
