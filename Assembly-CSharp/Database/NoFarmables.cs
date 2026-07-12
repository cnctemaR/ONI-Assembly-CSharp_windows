using System;
using STRINGS;

namespace Database
{
	public class NoFarmables : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				foreach (PlantablePlot plantablePlot in Components.PlantablePlots.GetItems(worldContainer.id))
				{
					if (plantablePlot.Occupant != null)
					{
						Tag[] possibleDepositObjectTags = plantablePlot.possibleDepositObjectTags;
						for (int i = 0; i < possibleDepositObjectTags.Length; i++)
						{
							if (possibleDepositObjectTags[i] != GameTags.DecorSeed)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !this.Success();
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_FARM_TILES;
		}
	}
}
