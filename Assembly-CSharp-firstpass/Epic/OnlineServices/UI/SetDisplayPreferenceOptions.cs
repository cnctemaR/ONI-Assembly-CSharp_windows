using System;

namespace Epic.OnlineServices.UI
{
	public class SetDisplayPreferenceOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public NotificationLocation NotificationLocation { get; set; }
	}
}
