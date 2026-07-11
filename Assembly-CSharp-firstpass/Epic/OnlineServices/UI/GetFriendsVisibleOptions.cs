using System;

namespace Epic.OnlineServices.UI
{
	public class GetFriendsVisibleOptions
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
