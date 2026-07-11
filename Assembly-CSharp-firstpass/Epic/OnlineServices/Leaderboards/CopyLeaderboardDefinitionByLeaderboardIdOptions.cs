using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardDefinitionByLeaderboardIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string LeaderboardId { get; set; }
	}
}
