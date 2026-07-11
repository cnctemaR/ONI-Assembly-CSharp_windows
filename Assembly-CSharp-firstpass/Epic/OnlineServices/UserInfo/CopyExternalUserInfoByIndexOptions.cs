using System;

namespace Epic.OnlineServices.UserInfo
{
	public class CopyExternalUserInfoByIndexOptions
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

		public uint Index { get; set; }
	}
}
