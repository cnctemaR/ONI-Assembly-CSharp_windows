using System;

namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionV2ByAchievementIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public string AchievementId { get; set; }
	}
}
