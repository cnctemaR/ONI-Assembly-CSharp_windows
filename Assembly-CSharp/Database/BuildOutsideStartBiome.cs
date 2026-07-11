using System;
using System.IO;
using Delaunay.Geo;
using Klei;
using ProcGen;

namespace Database
{
	public class BuildOutsideStartBiome : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
			for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
			{
				WorldDetailSave.OverworldCell overworldCell = worldDetailSave.overworldCells[i];
				if (overworldCell.tags != null && !overworldCell.tags.Contains(WorldGenTags.StartWorld))
				{
					Polygon poly = overworldCell.poly;
					foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
					{
						KPrefabID component = buildingComplete.GetComponent<KPrefabID>();
						if (!component.HasTag(GameTags.TemplateBuilding) && poly.PointInPolygon(buildingComplete.transform.GetPosition()))
						{
							Game.Instance.unlocks.Unlock("buildoutsidestartingbiome");
							return true;
						}
					}
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
