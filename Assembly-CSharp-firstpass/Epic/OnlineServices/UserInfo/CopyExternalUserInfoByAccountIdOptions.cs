using System;

namespace Epic.OnlineServices.UserInfo
{
	public class CopyExternalUserInfoByAccountIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }

		public string AccountId { get; set; }
	}
}
