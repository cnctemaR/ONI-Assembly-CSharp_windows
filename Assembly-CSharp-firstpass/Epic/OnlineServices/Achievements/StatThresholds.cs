using System;

namespace Epic.OnlineServices.Achievements
{
	public class StatThresholds
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Name { get; set; }

		public int Threshold { get; set; }
	}
}
