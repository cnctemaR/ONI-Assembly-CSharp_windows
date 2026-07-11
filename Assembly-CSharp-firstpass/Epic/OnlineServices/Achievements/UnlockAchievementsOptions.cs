using System;

namespace Epic.OnlineServices.Achievements
{
	public class UnlockAchievementsOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId UserId { get; set; }

		public string[] AchievementIds { get; set; }
	}
}
