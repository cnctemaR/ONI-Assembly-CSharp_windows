using System;
using STRINGS;

namespace Database
{
	public class DefeatPrehistoricAsteroid : VictoryColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			int num = 1000;
			int num2 = (complete ? num : 0);
			GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
			if (gameplayEventInstance != null)
			{
				LargeImpactorEvent.StatesInstance statesInstance = (LargeImpactorEvent.StatesInstance)gameplayEventInstance.smi;
				if (statesInstance != null && statesInstance.impactorInstance != null)
				{
					LargeImpactorStatus.Instance smi = statesInstance.impactorInstance.GetSMI<LargeImpactorStatus.Instance>();
					num = smi.def.MAX_HEALTH;
					num2 = num - smi.Health;
				}
			}
			return GameUtil.SafeStringFormat(COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.REQUIREMENT_DESCRIPTION, new object[]
			{
				GameUtil.GetFormattedInt((float)num2, GameUtil.TimeSlice.None),
				GameUtil.GetFormattedInt((float)num, GameUtil.TimeSlice.None)
			});
		}

		public override string Description()
		{
			return COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.DESCRIPTION;
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.largeImpactorState == ColonyAchievementTracker.LargeImpactorState.Defeated;
		}

		public override bool Fail()
		{
			return SaveGame.Instance.ColonyAchievementTracker.largeImpactorState == ColonyAchievementTracker.LargeImpactorState.Landed;
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.REQUIREMENT_NAME;
		}
	}
}
