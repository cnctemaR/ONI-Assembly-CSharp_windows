using System;
using Klei;
using ProcGen;
using STRINGS;

namespace Database
{
	public class BuildOutsideStartBiome : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				if (!buildingComplete.GetComponent<KPrefabID>().HasTag(GameTags.TemplateBuilding))
				{
					for (int i = 0; i < clusterDetailSave.overworldCells.Count; i++)
					{
						WorldDetailSave.OverworldCell overworldCell = clusterDetailSave.overworldCells[i];
						if (overworldCell.tags != null && !overworldCell.tags.Contains(WorldGenTags.StartWorld) && overworldCell.poly.PointInPolygon(buildingComplete.transform.GetPosition()))
						{
							Game.Instance.unlocks.Unlock("buildoutsidestartingbiome", true);
							return true;
						}
					}
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_OUTSIDE_START;
		}
	}
}
