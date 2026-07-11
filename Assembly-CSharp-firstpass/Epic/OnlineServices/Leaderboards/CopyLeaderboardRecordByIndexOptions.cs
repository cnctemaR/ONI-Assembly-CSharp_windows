using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardRecordByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public uint LeaderboardRecordIndex { get; set; }
	}
}
