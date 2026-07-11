using System;

namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint AchievementIndex { get; set; }
	}
}
