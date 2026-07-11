using System;

namespace UnityEngine.UIElements.UIR
{
	internal struct BMPAlloc
	{
		public bool Equals(BMPAlloc other)
		{
			return this.page == other.page && this.pageLine == other.pageLine && this.bitIndex == other.bitIndex;
		}

		public bool IsValid()
		{
			return this.page >= 0;
		}

		public static readonly BMPAlloc Invalid = new BMPAlloc
		{
			page = -1
		};

		public int page;

		public ushort pageLine;

		public byte bitIndex;

		public byte owned;
	}
}
