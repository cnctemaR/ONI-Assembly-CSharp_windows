using System;

namespace Mono
{
	internal struct CFRange
	{
		public CFRange(int loc, int len)
		{
			this.Location = (IntPtr)loc;
			this.Length = (IntPtr)len;
		}

		public IntPtr Location;

		public IntPtr Length;
	}
}
