using System;

namespace Epic.OnlineServices.Lobby
{
	public class UpdateLobbyOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public LobbyModification LobbyModificationHandle { get; set; }
	}
}
