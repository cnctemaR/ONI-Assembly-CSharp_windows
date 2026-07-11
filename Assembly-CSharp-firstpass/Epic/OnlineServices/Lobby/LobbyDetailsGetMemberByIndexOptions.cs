using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsGetMemberByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint MemberIndex { get; set; }
	}
}
