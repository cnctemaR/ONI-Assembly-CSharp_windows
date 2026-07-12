using System;
using STRINGS;

namespace Database
{
	public class ClearBlockedGeothermalVent : VictoryColonyAchievementRequirement
	{
		public override string Description()
		{
			return this.GetProgress(this.Success());
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.UNBLOCK_VENT_TITLE;
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.GeothermalClearedEntombedVent;
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.UNBLOCK_VENT_DESCRIPTION;
		}
	}
}
