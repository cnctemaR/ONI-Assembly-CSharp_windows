using System;

namespace Epic.OnlineServices.Lobby
{
	public class GetInviteCountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }
	}
}
