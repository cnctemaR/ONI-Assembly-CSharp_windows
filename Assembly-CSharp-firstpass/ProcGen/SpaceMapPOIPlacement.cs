using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class SpaceMapPOIPlacement
	{
		public List<string> pois { get; private set; }

		public int numToSpawn { get; set; }

		public MinMaxI allowedRings { get; set; }

		public SpaceMapPOIPlacement()
		{
			this.allowedRings = new MinMaxI(0, 9999);
		}

		public bool avoidClumping { get; set; }

		public bool canSpawnDuplicates { get; set; }
	}
}
