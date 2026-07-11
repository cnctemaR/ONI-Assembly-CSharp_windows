using System;
using System.IO;

namespace Database
{
	public class CoolBuildingToXKelvin : ColonyAchievementRequirement
	{
		public CoolBuildingToXKelvin(int kelvinToCoolTo)
		{
			this.kelvinToCoolTo = kelvinToCoolTo;
		}

		public override bool Success()
		{
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				if (buildingComplete.GetComponent<PrimaryElement>().Temperature <= (float)this.kelvinToCoolTo)
				{
					return true;
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
			this.kelvinToCoolTo = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.kelvinToCoolTo);
		}

		private int kelvinToCoolTo;
	}
}
