using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetLobbyIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string LobbyId { get; set; }
	}
}
