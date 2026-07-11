using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetInvitesAllowedOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public bool InvitesAllowed { get; set; }
	}
}
