using System;

namespace UnityEngine.SocialPlatforms
{
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
