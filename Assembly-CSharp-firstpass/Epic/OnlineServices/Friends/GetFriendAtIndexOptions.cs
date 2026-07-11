using System;

namespace Epic.OnlineServices.Friends
{
	public class GetFriendAtIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public int Index { get; set; }
	}
}
