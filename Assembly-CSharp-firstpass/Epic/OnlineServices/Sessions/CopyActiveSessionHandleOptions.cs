using System;

namespace Epic.OnlineServices.Sessions
{
	public class CopyActiveSessionHandleOptions
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
