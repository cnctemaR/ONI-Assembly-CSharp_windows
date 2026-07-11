using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetPermissionLevelOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public OnlineSessionPermissionLevel PermissionLevel { get; set; }
	}
}
