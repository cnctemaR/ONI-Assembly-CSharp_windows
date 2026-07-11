using System;

namespace Epic.OnlineServices.UserInfo
{
	public class CopyUserInfoOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
