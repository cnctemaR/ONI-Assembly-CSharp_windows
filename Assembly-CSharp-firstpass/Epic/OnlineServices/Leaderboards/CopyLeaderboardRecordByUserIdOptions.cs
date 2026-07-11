using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardRecordByUserIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public ProductUserId UserId { get; set; }
	}
}
