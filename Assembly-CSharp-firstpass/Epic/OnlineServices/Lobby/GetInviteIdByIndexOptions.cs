using System;

namespace Epic.OnlineServices.Lobby
{
	public class GetInviteIdByIndexOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public uint Index { get; set; }
	}
}
