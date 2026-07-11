using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class ActivateLorePOI : ColonyAchievementRequirement
	{
		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override bool Success()
		{
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				KPrefabID component = buildingComplete.GetComponent<KPrefabID>();
				if (component.HasTag(GameTags.TemplateBuilding))
				{
					Unsealable component2 = buildingComplete.GetComponent<Unsealable>();
					if (component2 != null && component2.unsealed)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.INVESTIGATE_A_POI;
		}
	}
}
