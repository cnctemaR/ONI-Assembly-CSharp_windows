using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationSetPermissionLevelOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public LobbyPermissionLevel PermissionLevel { get; set; }
	}
}
