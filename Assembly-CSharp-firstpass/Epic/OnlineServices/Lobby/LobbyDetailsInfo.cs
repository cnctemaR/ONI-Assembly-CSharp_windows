using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsInfo
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string LobbyId { get; set; }

		public ProductUserId LobbyOwnerUserId { get; set; }

		public LobbyPermissionLevel PermissionLevel { get; set; }

		public uint AvailableSlots { get; set; }

		public uint MaxMembers { get; set; }

		public bool AllowInvites { get; set; }
	}
}
