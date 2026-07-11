using System;

namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionByAchievementIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string AchievementId { get; set; }
	}
}
