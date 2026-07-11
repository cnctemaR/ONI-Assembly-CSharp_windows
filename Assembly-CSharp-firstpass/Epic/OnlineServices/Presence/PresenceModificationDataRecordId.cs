using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationDataRecordId
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Key { get; set; }
	}
}
