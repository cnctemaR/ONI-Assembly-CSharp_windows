using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchCopySearchResultByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint SessionIndex { get; set; }
	}
}
