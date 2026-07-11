using System;

namespace Epic.OnlineServices.P2P
{
	public class AcceptConnectionOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public ProductUserId RemoteUserId { get; set; }

		public SocketId SocketId { get; set; }
	}
}
