using System;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("IAchievementDescription is deprecated and will be removed in a future release.", false)]
	public interface IAchievementDescription
	{
		string id { get; set; }

		string title { get; }

		Texture2D image { get; }

		string achievedDescription { get; }

		string unachievedDescription { get; }

		bool hidden { get; }

		int points { get; }
	}
}
