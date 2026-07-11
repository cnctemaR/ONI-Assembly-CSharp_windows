using System;

namespace Epic.OnlineServices.Lobby
{
	public class CopyLobbyDetailsHandleOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string LobbyId { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
