using System;

namespace Epic.OnlineServices.Friends
{
	public class QueryFriendsOptions
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
