using System;

namespace System
{
	internal struct SmallRect
	{
		public SmallRect(int left, int top, int right, int bottom)
		{
			this.Left = (short)left;
			this.Top = (short)top;
			this.Right = (short)right;
			this.Bottom = (short)bottom;
		}

		public short Left;

		public short Top;

		public short Right;

		public short Bottom;
	}
}
