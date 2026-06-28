using System;

namespace Rendering.World
{
	public struct Tile
	{
		public Tile(int idx, int tile_x, int tile_y, int mask_count)
		{
			this.Idx = idx;
			this.TileCells = new TileCells(tile_x, tile_y);
			this.MaskCount = mask_count;
		}

		public int Idx;

		public TileCells TileCells;

		public int MaskCount;
	}
}
