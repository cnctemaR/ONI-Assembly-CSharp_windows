using System;

namespace Klei
{
	public class WorldGenSave
	{
		public WorldGenSave()
		{
			this.data = new Data();
			this.stats = new WorldGenStats();
		}

		public Vector2I version;

		public WorldGenStats stats;

		public Data data;
	}
}
