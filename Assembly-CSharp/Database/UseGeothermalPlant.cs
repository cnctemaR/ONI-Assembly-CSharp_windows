using System;
using STRINGS;

namespace Database
{
	public class UseGeothermalPlant : VictoryColonyAchievementRequirement
	{
		public override string Description()
		{
			return this.GetProgress(this.Success());
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.ACTIVATE_PLANT_TITLE;
		}

		public override bool Success()
		{
			return SaveGame.Instance.ColonyAchievementTracker.GeothermalControllerHasVented;
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.ACTIVATE_PLANT_DESCRIPTION;
		}
	}
}
