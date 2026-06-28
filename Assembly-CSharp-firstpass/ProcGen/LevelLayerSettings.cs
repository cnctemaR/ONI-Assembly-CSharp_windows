using System;
using Klei;

namespace ProcGen
{
	public class LevelLayerSettings : YamlIO<LevelLayerSettings>
	{
		public LevelLayerSettings()
		{
			this.LevelLayers = new LevelLayer();
		}

		public LevelLayer LevelLayers { get; private set; }
	}
}
