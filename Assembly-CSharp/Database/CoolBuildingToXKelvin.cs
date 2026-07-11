using System;
using System.IO;
using STRINGS;

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

		public override string GetProgress(bool complete)
		{
			float num = float.MaxValue;
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				num = Math.Min(num, buildingComplete.GetComponent<PrimaryElement>().Temperature);
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.KELVIN_COOLING, num);
		}

		private int kelvinToCoolTo;
	}
}
