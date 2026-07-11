using System;

namespace Epic.OnlineServices.Achievements
{
	public class QueryPlayerAchievementsOptions
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
