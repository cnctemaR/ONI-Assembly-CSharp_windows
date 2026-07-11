using System;

namespace Epic.OnlineServices.P2P
{
	public class AddNotifyPeerConnectionClosedOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public SocketId SocketId { get; set; }
	}
}
