using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public static class Timeout
	{
		[ComVisible(false)]
		public static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

		public const int Infinite = -1;

		internal const uint UnsignedInfinite = 4294967295U;
	}
}
