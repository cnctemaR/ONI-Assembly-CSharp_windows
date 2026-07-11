using System;
using System.IO;
using KSerialization;

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
			bool flag = Db.Get().TechItems.IsTechItemComplete(this.upgradeBuilding.Name);
			if (flag)
			{
				bool flag2 = false;
				foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
				{
					KPrefabID component = buildingComplete.GetComponent<KPrefabID>();
					if (component.HasTag(this.basicBuilding))
					{
						return false;
					}
					if (component.HasTag(this.upgradeBuilding))
					{
						flag2 = true;
					}
				}
				return flag2;
			}
			return false;
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

		private Tag basicBuilding;

		private Tag upgradeBuilding;
	}
}
