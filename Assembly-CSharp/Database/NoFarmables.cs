using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class NoFarmables : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (PlantablePlot plantablePlot in Components.PlantablePlots.Items)
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
			return true;
		}

		public override bool Fail()
		{
			return !this.Success();
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_FARM_TILES;
		}
	}
}
