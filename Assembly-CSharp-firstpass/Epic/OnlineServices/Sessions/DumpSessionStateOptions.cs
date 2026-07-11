using System;

namespace Epic.OnlineServices.Sessions
{
	public class DumpSessionStateOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string SessionName { get; set; }
	}
}
