using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetHostAddressOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string HostAddress { get; set; }
	}
}
