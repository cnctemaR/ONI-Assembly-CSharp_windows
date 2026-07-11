using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationSetMaxMembersOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint MaxMembers { get; set; }
	}
}
