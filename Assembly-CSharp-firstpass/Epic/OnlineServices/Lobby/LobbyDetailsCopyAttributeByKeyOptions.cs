using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsCopyAttributeByKeyOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string AttrKey { get; set; }
	}
}
