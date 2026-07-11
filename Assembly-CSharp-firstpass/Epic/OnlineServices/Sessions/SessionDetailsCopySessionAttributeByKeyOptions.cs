using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionDetailsCopySessionAttributeByKeyOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string AttrKey { get; set; }
	}
}
