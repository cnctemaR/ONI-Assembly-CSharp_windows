using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;

namespace Database
{
	public class AtLeastOneBuildingForEachDupe : ColonyAchievementRequirement
	{
		public AtLeastOneBuildingForEachDupe(List<Tag> validBuildingTypes)
		{
			this.validBuildingTypes = validBuildingTypes;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				if (this.validBuildingTypes.Contains(buildingComplete.prefabid.PrefabTag))
				{
					num++;
					if (buildingComplete.prefabid.PrefabTag == "FlushToilet" || buildingComplete.prefabid.PrefabTag == "Outhouse")
					{
						return true;
					}
				}
			}
			return Components.LiveMinionIdentities.Items.Count > 0 && num >= Components.LiveMinionIdentities.Items.Count;
		}

		public override bool Fail()
		{
			return Components.LiveMinionIdentities.Items.Count <= 0;
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			this.validBuildingTypes = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.validBuildingTypes.Add(new Tag(text));
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.validBuildingTypes.Count);
			foreach (Tag tag in this.validBuildingTypes)
			{
				writer.WriteKleiString(tag.ToString());
			}
		}

		public override string GetProgress(bool complete)
		{
			if (this.validBuildingTypes.Contains("FlushToilet"))
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_TOILET;
			}
			if (complete)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_BED_PER_DUPLICANT;
			}
			int num = 0;
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				if (this.validBuildingTypes.Contains(buildingComplete.prefabid.PrefabTag))
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILING_BEDS, (!complete) ? num : Components.LiveMinionIdentities.Items.Count, Components.LiveMinionIdentities.Items.Count);
		}

		private List<Tag> validBuildingTypes = new List<Tag>();
	}
}
