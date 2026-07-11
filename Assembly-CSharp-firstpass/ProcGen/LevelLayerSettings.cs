using System;

namespace ProcGen
{
	public class LevelLayerSettings : IMerge<LevelLayerSettings>
	{
		public LevelLayerSettings()
		{
			this.LevelLayers = new LevelLayer();
		}

		public LevelLayer LevelLayers { get; private set; }

		public void Merge(LevelLayerSettings other)
		{
			this.LevelLayers.Merge(other.LevelLayers);
		}
	}
}
