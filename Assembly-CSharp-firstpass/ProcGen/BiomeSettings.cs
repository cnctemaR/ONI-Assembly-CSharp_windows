using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class BiomeSettings : YamlIO<BiomeSettings>
	{
		public BiomeSettings()
		{
			this.TerrainBiomeLookupTable = new Dictionary<string, ElementBandConfiguration>();
		}

		public Dictionary<string, ElementBandConfiguration> TerrainBiomeLookupTable { get; private set; }

		public string[] GetNames()
		{
			string[] array = new string[this.TerrainBiomeLookupTable.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair in this.TerrainBiomeLookupTable)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}
	}
}
