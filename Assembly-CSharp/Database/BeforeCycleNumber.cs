using System;
using System.IO;
using STRINGS;
using UnityEngine;

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

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REMAINING_CYCLES, Mathf.Max(this.cycleNumber - GameClock.Instance.GetCycle(), 0), this.cycleNumber);
		}

		private int cycleNumber;
	}
}
