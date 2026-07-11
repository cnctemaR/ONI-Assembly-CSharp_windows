using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardDefinitionByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint LeaderboardIndex { get; set; }
	}
}
