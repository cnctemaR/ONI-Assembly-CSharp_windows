using System;

namespace Epic.OnlineServices.Presence
{
	public class SetPresenceOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public EpicAccountId LocalUserId { get; set; }

		public PresenceModification PresenceModificationHandle { get; set; }
	}
}
