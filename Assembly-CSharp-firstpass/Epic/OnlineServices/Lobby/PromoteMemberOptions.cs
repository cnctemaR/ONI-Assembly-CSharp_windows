using System;

namespace Epic.OnlineServices.Lobby
{
	public class PromoteMemberOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string LobbyId { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public ProductUserId TargetUserId { get; set; }
	}
}
