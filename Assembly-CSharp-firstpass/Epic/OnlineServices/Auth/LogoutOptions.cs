using System;

namespace Epic.OnlineServices.Auth
{
	public class LogoutOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }
	}
}
