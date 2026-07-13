using System;
using STRINGS;

namespace Database
{
	public class NoDuplicantsCanDie : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.NO_DUPES_HAVE_DIED.REQUIREMENT_NAME;
		}

		public override bool Success()
		{
			return !SaveGame.Instance.ColonyAchievementTracker.HasAnyDupeDied;
		}

		public override bool Fail()
		{
			return SaveGame.Instance.ColonyAchievementTracker.HasAnyDupeDied;
		}
	}
}
