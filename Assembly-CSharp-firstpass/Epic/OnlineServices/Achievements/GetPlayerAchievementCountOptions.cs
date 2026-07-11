using System;

namespace Epic.OnlineServices.Achievements
{
	public class GetPlayerAchievementCountOptions
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
