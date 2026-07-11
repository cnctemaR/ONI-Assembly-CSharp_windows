using System;

namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionV2ByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public uint AchievementIndex { get; set; }
	}
}
