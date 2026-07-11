using System;

namespace Epic.OnlineServices.Lobby
{
	public class CreateLobbySearchOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint MaxResults { get; set; }
	}
}
