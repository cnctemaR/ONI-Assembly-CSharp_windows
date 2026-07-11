using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardUserScoreByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint LeaderboardUserScoreIndex { get; set; }

		public string StatName { get; set; }
	}
}
