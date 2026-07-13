using System;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("Range is deprecated and will be removed in a future release.", false)]
	public struct Range
	{
		public Range(int fromValue, int valueCount)
		{
			this.from = fromValue;
			this.count = valueCount;
		}

		public int from;

		public int count;
	}
}
