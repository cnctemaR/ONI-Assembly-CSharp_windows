using System;
using STRINGS;

namespace Database
{
	public class AnalyzeSeed : ColonyAchievementRequirement
	{
		public AnalyzeSeed(string seedname)
		{
			this.seedName = seedname;
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ANALYZE_SEED, this.seedName.ProperName());
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.analyzedSeeds.Contains(this.seedName);
		}

		private string seedName;
	}
}
