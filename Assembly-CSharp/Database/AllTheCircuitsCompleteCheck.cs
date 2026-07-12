using System;
using STRINGS;

namespace Database
{
	public class AllTheCircuitsCompleteCheck : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MVB_DESCRIPTION, 8);
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.fullyBoostedBionic;
		}
	}
}
