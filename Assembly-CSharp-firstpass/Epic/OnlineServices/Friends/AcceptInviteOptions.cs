using System;

namespace Epic.OnlineServices.Friends
{
	public class AcceptInviteOptions
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
	}
}
