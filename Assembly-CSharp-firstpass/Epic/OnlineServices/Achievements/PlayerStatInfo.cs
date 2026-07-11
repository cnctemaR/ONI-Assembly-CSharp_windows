using System;

namespace Epic.OnlineServices.Achievements
{
	public class PlayerStatInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Name { get; set; }

		public int CurrentValue { get; set; }

		public int ThresholdValue { get; set; }
	}
}
