using System;

namespace Epic.OnlineServices.UI
{
	public class HideFriendsOptions
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
