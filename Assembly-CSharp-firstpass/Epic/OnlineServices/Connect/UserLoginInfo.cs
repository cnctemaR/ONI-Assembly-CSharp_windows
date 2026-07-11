using System;

namespace Epic.OnlineServices.Connect
{
	public class UserLoginInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string DisplayName { get; set; }
	}
}
