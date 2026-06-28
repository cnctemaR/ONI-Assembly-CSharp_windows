using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class TerrainBiomeSettings : YamlIO<TerrainBiomeSettings>
	{
		public TerrainBiomeSettings()
		{
			this.TerrainBiomeLookupTable = new Dictionary<string, Biome>();
		}

		public Dictionary<string, Biome> TerrainBiomeLookupTable { get; private set; }

		public List<string> LoadBiomeFiles { get; private set; }

		public string[] GetNames()
		{
			string[] array = new string[this.TerrainBiomeLookupTable.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Biome> keyValuePair in this.TerrainBiomeLookupTable)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}
	}
}
