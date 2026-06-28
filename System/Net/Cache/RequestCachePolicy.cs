using System;

namespace System.Net.Cache
{
	public class RequestCachePolicy
	{
		public RequestCachePolicy()
		{
		}

		public RequestCachePolicy(RequestCacheLevel level)
		{
			this.level = level;
		}

		public RequestCacheLevel Level
		{
			get
			{
				return this.level;
			}
		}

		[global::System.MonoTODO]
		public override string ToString()
		{
			throw new NotImplementedException();
		}

		private RequestCacheLevel level;
	}
}
