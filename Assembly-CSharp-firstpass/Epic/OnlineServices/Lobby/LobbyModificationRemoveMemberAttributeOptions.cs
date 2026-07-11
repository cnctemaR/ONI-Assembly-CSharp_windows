using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationRemoveMemberAttributeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string Key { get; set; }
	}
}
