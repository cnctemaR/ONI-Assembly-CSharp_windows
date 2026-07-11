using System;

namespace Epic.OnlineServices.Achievements
{
	public class UnlockedAchievement
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string AchievementId { get; set; }

		public DateTimeOffset? UnlockTime { get; set; }
	}
}
