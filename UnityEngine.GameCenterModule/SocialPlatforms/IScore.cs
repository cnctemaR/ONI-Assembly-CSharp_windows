using System;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("IScore is deprecated and will be removed in a future release.", false)]
	public interface IScore
	{
		void ReportScore(Action<bool> callback);

		string leaderboardID { get; set; }

		long value { get; set; }

		DateTime date { get; }

		string formattedValue { get; }

		string userID { get; }

		int rank { get; }
	}
}
