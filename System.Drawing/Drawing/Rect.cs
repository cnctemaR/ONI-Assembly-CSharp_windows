using System;

namespace System.Drawing
{
	internal struct Rect
	{
		public Rect(float x, float y, float width, float height)
		{
			this.origin.x = x;
			this.origin.y = y;
			this.size.width = width;
			this.size.height = height;
		}

		public CGPoint origin;

		public CGSize size;
	}
}
