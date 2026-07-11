using System;

namespace Epic.OnlineServices.Lobby
{
	public class CopyLobbyDetailsHandleByInviteIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string InviteId { get; set; }
	}
}
