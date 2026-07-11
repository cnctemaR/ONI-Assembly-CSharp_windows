using System;
using System.IO;
using KSerialization;
using STRINGS;

namespace Database
{
	public class UpgradeAllBasicBuildings : ColonyAchievementRequirement
	{
		public UpgradeAllBasicBuildings(Tag basicBuilding, Tag upgradeBuilding)
		{
			this.basicBuilding = basicBuilding;
			this.upgradeBuilding = upgradeBuilding;
		}

		public override bool Success()
		{
			bool flag = false;
			foreach (IBasicBuilding basicBuilding in Components.BasicBuildings.Items)
			{
				KPrefabID component = basicBuilding.transform.GetComponent<KPrefabID>();
				if (component.HasTag(this.basicBuilding))
				{
					return false;
				}
				if (component.HasTag(this.upgradeBuilding))
				{
					flag = true;
				}
			}
			return flag;
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.basicBuilding = new Tag(text);
			string text2 = reader.ReadKleiString();
			this.upgradeBuilding = new Tag(text2);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(this.basicBuilding.ToString());
			writer.WriteKleiString(this.upgradeBuilding.ToString());
		}

		public override string GetProgress(bool complete)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(this.basicBuilding.Name);
			BuildingDef buildingDef2 = Assets.GetBuildingDef(this.upgradeBuilding.Name);
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.UPGRADE_ALL_BUILDINGS, buildingDef.Name, buildingDef2.Name);
		}

		private Tag basicBuilding;

		private Tag upgradeBuilding;
	}
}
