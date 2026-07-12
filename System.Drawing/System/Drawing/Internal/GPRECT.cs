using System;

namespace System.Drawing.Internal
{
	internal struct GPRECT
	{
		internal GPRECT(int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		internal GPRECT(Rectangle rect)
		{
			this.X = rect.X;
			this.Y = rect.Y;
			this.Width = rect.Width;
			this.Height = rect.Height;
		}

		internal int X;

		internal int Y;

		internal int Width;

		internal int Height;
	}
}
