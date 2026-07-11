using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsCopyAttributeByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint AttrIndex { get; set; }
	}
}
