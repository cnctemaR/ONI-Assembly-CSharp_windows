using System;

namespace Epic.OnlineServices.Achievements
{
	public class CopyUnlockedAchievementByAchievementIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId UserId { get; set; }

		public string AchievementId { get; set; }
	}
}
