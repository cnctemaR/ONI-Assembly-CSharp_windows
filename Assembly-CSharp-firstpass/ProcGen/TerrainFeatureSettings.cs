using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class TerrainFeatureSettings : YamlIO<TerrainFeatureSettings>
	{
		public TerrainFeatureSettings()
		{
			this.TerrainFeatures = new Dictionary<string, TerrainFeature>();
		}

		public Dictionary<string, TerrainFeature> TerrainFeatures { get; private set; }

		public string[] GetNames()
		{
			string[] array = new string[this.TerrainFeatures.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, TerrainFeature> keyValuePair in this.TerrainFeatures)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}
	}
}
