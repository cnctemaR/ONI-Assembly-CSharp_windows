using System;

namespace TiledSharp
{
	public class TmxLayerTile
	{
		public int Gid { get; private set; }

		public int X { get; private set; }

		public int Y { get; private set; }

		public bool HorizontalFlip { get; private set; }

		public bool VerticalFlip { get; private set; }

		public bool DiagonalFlip { get; private set; }

		public TmxLayerTile(uint id, int x, int y)
		{
			this.X = x;
			this.Y = y;
			this.HorizontalFlip = (id & 2147483648U) != 0U;
			this.VerticalFlip = (id & 1073741824U) != 0U;
			this.DiagonalFlip = (id & 536870912U) != 0U;
			uint num = id & 536870911U;
			this.Gid = (int)num;
		}

		private const uint FLIPPED_HORIZONTALLY_FLAG = 2147483648U;

		private const uint FLIPPED_VERTICALLY_FLAG = 1073741824U;

		private const uint FLIPPED_DIAGONALLY_FLAG = 536870912U;
	}
}
