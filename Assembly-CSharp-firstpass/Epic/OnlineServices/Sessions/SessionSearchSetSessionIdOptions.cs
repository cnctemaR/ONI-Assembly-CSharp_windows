using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchSetSessionIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string SessionId { get; set; }
	}
}
