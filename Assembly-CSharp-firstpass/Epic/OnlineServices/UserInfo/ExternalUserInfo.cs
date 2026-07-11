using System;

namespace Epic.OnlineServices.UserInfo
{
	public class ExternalUserInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ExternalAccountType AccountType { get; set; }

		public string AccountId { get; set; }

		public string DisplayName { get; set; }
	}
}
