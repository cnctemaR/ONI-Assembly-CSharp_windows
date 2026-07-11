using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetJoinInfoOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string JoinInfo { get; set; }
	}
}
