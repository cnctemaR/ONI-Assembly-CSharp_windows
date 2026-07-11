using System;

namespace Epic.OnlineServices.Achievements
{
	public class GetUnlockedAchievementCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId UserId { get; set; }
	}
}
