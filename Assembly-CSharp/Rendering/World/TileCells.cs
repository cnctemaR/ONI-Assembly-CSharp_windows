using System;

namespace Rendering.World
{
	public struct TileCells
	{
		public TileCells(int tile_x, int tile_y)
		{
			int num = Grid.WidthInCells - 1;
			int num2 = Grid.HeightInCells - 1;
			this.Cell0 = Grid.XYToCell(Math.Min(Math.Max(tile_x - 1, 0), num), Math.Min(Math.Max(tile_y - 1, 0), num2));
			this.Cell1 = Grid.XYToCell(Math.Min(tile_x, num), Math.Min(Math.Max(tile_y - 1, 0), num2));
			this.Cell2 = Grid.XYToCell(Math.Min(Math.Max(tile_x - 1, 0), num), Math.Min(tile_y, num2));
			this.Cell3 = Grid.XYToCell(Math.Min(tile_x, num), Math.Min(tile_y, num2));
		}

		public int Cell0;

		public int Cell1;

		public int Cell2;

		public int Cell3;
	}
}
