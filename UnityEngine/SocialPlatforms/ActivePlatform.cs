using System;

namespace UnityEngine.SocialPlatforms
{
	internal static class ActivePlatform
	{
		internal static ISocialPlatform Instance
		{
			get
			{
				if (ActivePlatform._active == null)
				{
					ActivePlatform._active = ActivePlatform.SelectSocialPlatform();
				}
				return ActivePlatform._active;
			}
			set
			{
				ActivePlatform._active = value;
			}
		}

		private static ISocialPlatform SelectSocialPlatform()
		{
			return new Local();
		}

		private static ISocialPlatform _active;
	}
}
