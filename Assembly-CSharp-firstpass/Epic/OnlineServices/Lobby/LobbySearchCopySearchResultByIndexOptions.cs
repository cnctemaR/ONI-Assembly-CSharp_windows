using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchCopySearchResultByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint LobbyIndex { get; set; }
	}
}
