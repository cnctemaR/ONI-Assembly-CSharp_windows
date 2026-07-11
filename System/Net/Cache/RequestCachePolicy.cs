using System;

namespace System.Net.Cache
{
	public class RequestCachePolicy
	{
		public RequestCachePolicy()
			: this(RequestCacheLevel.Default)
		{
		}

		public RequestCachePolicy(RequestCacheLevel level)
		{
			if (level < RequestCacheLevel.Default || level > RequestCacheLevel.NoCacheNoStore)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			this.m_Level = level;
		}

		public RequestCacheLevel Level
		{
			get
			{
				return this.m_Level;
			}
		}

		public override string ToString()
		{
			return "Level:" + this.m_Level.ToString();
		}

		private RequestCacheLevel m_Level;
	}
}
