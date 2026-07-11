using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetStatusOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public Status Status { get; set; }
	}
}
