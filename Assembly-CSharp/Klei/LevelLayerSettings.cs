using System;

namespace Klei
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
