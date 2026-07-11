using System;

namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetMaxPlayersOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint MaxPlayers { get; set; }
	}
}
