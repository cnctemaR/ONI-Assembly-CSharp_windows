using System;

namespace System.Drawing
{
	internal struct CGRect64
	{
		public CGRect64(double x, double y, double width, double height)
		{
			this.origin.x = x;
			this.origin.y = y;
			this.size.width = width;
			this.size.height = height;
		}

		public CGPoint64 origin;

		public CGSize64 size;
	}
}
