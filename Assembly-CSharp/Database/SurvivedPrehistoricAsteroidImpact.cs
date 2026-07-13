using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class SurvivedPrehistoricAsteroidImpact : ColonyAchievementRequirement
	{
		public SurvivedPrehistoricAsteroidImpact(int requiredCyclesAfterImpact)
		{
			this.requiredCyclesAfterImpact = requiredCyclesAfterImpact;
		}

		public override string GetProgress(bool complete)
		{
			int num = (complete ? this.requiredCyclesAfterImpact : 0);
			if (!complete && SaveGame.Instance.ColonyAchievementTracker.largeImpactorLandedCycle >= 0)
			{
				num = Mathf.Clamp(GameClock.Instance.GetCycle() - SaveGame.Instance.ColonyAchievementTracker.largeImpactorLandedCycle, 0, this.requiredCyclesAfterImpact);
			}
			return GameUtil.SafeStringFormat(COLONY_ACHIEVEMENTS.ASTEROID_SURVIVED.REQUIREMENT_DESCRIPTION, new object[]
			{
				GameUtil.GetFormattedInt((float)num, GameUtil.TimeSlice.None),
				GameUtil.GetFormattedInt((float)this.requiredCyclesAfterImpact, GameUtil.TimeSlice.None)
			});
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.largeImpactorLandedCycle >= 0 && GameClock.Instance.GetCycle() - SaveGame.Instance.ColonyAchievementTracker.largeImpactorLandedCycle >= this.requiredCyclesAfterImpact;
		}

		public override bool Fail()
		{
			return SaveGame.Instance.ColonyAchievementTracker.largeImpactorState == ColonyAchievementTracker.LargeImpactorState.Defeated;
		}

		private int requiredCyclesAfterImpact;
	}
}
