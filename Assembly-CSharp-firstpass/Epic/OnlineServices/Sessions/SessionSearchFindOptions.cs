using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchFindOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public ProductUserId LocalUserId { get; set; }
	}
}
