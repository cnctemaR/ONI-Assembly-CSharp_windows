using System;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("ILeaderboard is deprecated and will be removed in a future release.", false)]
	public interface ILeaderboard
	{
		void SetUserFilter(string[] userIDs);

		void LoadScores(Action<bool> callback);

		bool loading { get; }

		string id { get; set; }

		UserScope userScope { get; set; }

		Range range { get; set; }

		TimeScope timeScope { get; set; }

		IScore localUserScore { get; }

		uint maxRange { get; }

		IScore[] scores { get; }

		string title { get; }
	}
}
