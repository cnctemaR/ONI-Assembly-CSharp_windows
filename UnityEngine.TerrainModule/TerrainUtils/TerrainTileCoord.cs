using System;

namespace UnityEngine.TerrainUtils
{
	public readonly struct TerrainTileCoord
	{
		public TerrainTileCoord(int tileX, int tileZ)
		{
			this.tileX = tileX;
			this.tileZ = tileZ;
		}

		public readonly int tileX;

		public readonly int tileZ;
	}
}
