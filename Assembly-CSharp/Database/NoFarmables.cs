using System;
using System.IO;

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
					foreach (Tag tag in plantablePlot.possibleDepositObjectTags)
					{
						if (tag != GameTags.DecorSeed)
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
	}
}
