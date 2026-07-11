using System;

namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleForPresenceOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }
	}
}
