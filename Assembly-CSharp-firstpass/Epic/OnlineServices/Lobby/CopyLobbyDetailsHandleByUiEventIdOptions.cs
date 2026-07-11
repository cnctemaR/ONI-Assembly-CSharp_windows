using System;

namespace Epic.OnlineServices.Lobby
{
	public class CopyLobbyDetailsHandleByUiEventIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ulong UiEventId { get; set; }
	}
}
