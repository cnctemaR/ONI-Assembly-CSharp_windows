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
			return BuildingComplete.MinKelvinSeen <= (float)this.kelvinToCoolTo;
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
			float minKelvinSeen = BuildingComplete.MinKelvinSeen;
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.KELVIN_COOLING, minKelvinSeen);
		}

		private int kelvinToCoolTo;
	}
}
