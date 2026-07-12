using System;
using STRINGS;

namespace Database
{
	public class DiscoverGeothermalFacility : VictoryColonyAchievementRequirement
	{
		public override string Description()
		{
			return this.GetProgress(this.Success());
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.DISCOVER_GEOTHERMAL_FACILITY_DESCRIPTION;
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.REQUIREMENTS.DISCOVER_GEOTHERMAL_FACILITY_TITLE;
		}

		public override bool Success()
		{
			return GeothermalPlantComponent.GeothermalFacilityDiscovered();
		}
	}
}
