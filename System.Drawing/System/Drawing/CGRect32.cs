using System;

namespace System.Drawing
{
	internal struct CGRect32
	{
		public CGRect32(float x, float y, float width, float height)
		{
			this.origin.x = x;
			this.origin.y = y;
			this.size.width = width;
			this.size.height = height;
		}

		public CGPoint32 origin;

		public CGSize32 size;
	}
}
