using System;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("IUserProfile is deprecated and will be removed in a future release.", false)]
	public interface IUserProfile
	{
		string userName { get; }

		string id { get; }

		bool isFriend { get; }

		UserState state { get; }

		Texture2D image { get; }
	}
}
