using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class LeaderboardUserScore
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId UserId { get; set; }

		public int Score { get; set; }
	}
}
