using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationDeleteDataOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public PresenceModificationDataRecordId[] Records { get; set; }
	}
}
