using System;
using STRINGS;

namespace Database
{
	public class ReachedSpace : VictoryColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public ReachedSpace(SpaceDestinationType destinationType = null)
		{
			this.destinationType = destinationType;
		}

		public override string Name()
		{
			if (this.destinationType != null)
			{
				return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION, this.destinationType.Name);
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION;
		}

		public override string Description()
		{
			if (this.destinationType != null)
			{
				return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION_DESCRIPTION, this.destinationType.Name);
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION;
		}

		public override bool Success()
		{
			foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
			{
				if (spacecraft.state != Spacecraft.MissionState.Grounded && spacecraft.state != Spacecraft.MissionState.Destroyed)
				{
					SpaceDestination destination = SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.savedSpacecraftDestinations[spacecraft.id]);
					if (this.destinationType == null || destination.GetDestinationType() == this.destinationType)
					{
						if (this.destinationType == Db.Get().SpaceDestinationTypes.Wormhole)
						{
							Game.Instance.unlocks.Unlock("temporaltear", true);
						}
						return true;
					}
				}
			}
			return SpacecraftManager.instance.hasVisitedWormHole;
		}

		public void Deserialize(IReader reader)
		{
			if (reader.ReadByte() <= 0)
			{
				string text = reader.ReadKleiString();
				this.destinationType = Db.Get().SpaceDestinationTypes.Get(text);
			}
		}

		public override string GetProgress(bool completed)
		{
			if (this.destinationType == null)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET;
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET_TO_WORMHOLE;
		}

		private SpaceDestinationType destinationType;
	}
}
