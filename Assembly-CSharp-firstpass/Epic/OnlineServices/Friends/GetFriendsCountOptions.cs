using System;

namespace Epic.OnlineServices.Friends
{
	public class GetFriendsCountOptions
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
