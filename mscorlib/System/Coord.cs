using System;

namespace System
{
	internal struct Coord
	{
		public Coord(int x, int y)
		{
			this.X = (short)x;
			this.Y = (short)y;
		}

		public short X;

		public short Y;
	}
}
