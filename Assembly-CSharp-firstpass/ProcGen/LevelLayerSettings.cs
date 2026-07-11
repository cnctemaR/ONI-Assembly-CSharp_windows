using System;

namespace ProcGen
{
	public class LevelLayerSettings : IMerge<LevelLayerSettings>
	{
		public LevelLayer LevelLayers { get; private set; }

		public LevelLayerSettings()
		{
			this.LevelLayers = new LevelLayer();
		}

		public void Merge(LevelLayerSettings other)
		{
			this.LevelLayers.Merge(other.LevelLayers);
		}
	}
}
