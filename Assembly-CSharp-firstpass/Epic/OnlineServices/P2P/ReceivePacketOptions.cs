using System;

namespace Epic.OnlineServices.P2P
{
	public class ReceivePacketOptions
	{
		public int ApiVersion
		{
			get
			{
				return 2;
			}
		}

		public ProductUserId LocalUserId { get; set; }

		public uint MaxDataSizeBytes { get; set; }

		public byte? RequestedChannel { get; set; }
	}
}
