using System;

namespace System.Net.Sockets
{
	[Serializable]
	public struct SocketInformation
	{
		public SocketInformationOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public byte[] ProtocolInformation
		{
			get
			{
				return this.protocol_info;
			}
			set
			{
				this.protocol_info = value;
			}
		}

		private SocketInformationOptions options;

		private byte[] protocol_info;
	}
}
