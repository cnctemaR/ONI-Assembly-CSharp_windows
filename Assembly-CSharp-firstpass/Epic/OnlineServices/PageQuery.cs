using System;

namespace Epic.OnlineServices
{
	public class PageQuery
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public int StartIndex { get; set; }

		public int MaxCount { get; set; }
	}
}
