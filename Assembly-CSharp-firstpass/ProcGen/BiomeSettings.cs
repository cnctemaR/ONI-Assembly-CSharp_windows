using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class BiomeSettings : IMerge<BiomeSettings>
	{
		public ComposableDictionary<string, ElementBandConfiguration> TerrainBiomeLookupTable { get; private set; }

		public BiomeSettings()
		{
			this.TerrainBiomeLookupTable = new ComposableDictionary<string, ElementBandConfiguration>();
		}

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

		public BiomeSettings Merge(BiomeSettings other)
		{
			this.TerrainBiomeLookupTable.Merge(other.TerrainBiomeLookupTable);
			return this;
		}
	}
}
