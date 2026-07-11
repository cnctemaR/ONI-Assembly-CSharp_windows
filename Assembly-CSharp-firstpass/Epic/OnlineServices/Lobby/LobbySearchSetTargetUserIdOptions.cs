using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetTargetUserIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId TargetUserId { get; set; }
	}
}
