using System;

namespace Epic.OnlineServices.Sessions
{
	public class StartSessionOptions
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
