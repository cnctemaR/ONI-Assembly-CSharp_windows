using System;
using STRINGS;

namespace Database
{
	public class EfficientDataMiningCheck : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.DATA_DRIVEN_DESCRIPTION;
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.efficientlyGatheredData;
		}
	}
}
