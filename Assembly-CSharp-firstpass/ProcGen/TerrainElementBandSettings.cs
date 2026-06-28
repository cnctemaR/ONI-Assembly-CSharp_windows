using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class TerrainElementBandSettings : YamlIO<TerrainElementBandSettings>
	{
		public TerrainElementBandSettings()
		{
			this.BiomeBackgroundElementBandConfigurations = new Dictionary<string, ElementBandConfiguration>();
		}

		public Dictionary<string, ElementBandConfiguration> BiomeBackgroundElementBandConfigurations { get; private set; }

		public string[] GetNames()
		{
			string[] array = new string[this.BiomeBackgroundElementBandConfigurations.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair in this.BiomeBackgroundElementBandConfigurations)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}
	}
}
