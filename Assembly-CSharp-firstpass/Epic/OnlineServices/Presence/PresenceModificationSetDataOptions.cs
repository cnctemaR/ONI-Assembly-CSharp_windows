using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetDataOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public DataRecord[] Records { get; set; }
	}
}
