using System;

namespace System.Net.NetworkInformation
{
	internal class SystemTcpConnectionInformation : TcpConnectionInformation
	{
		public SystemTcpConnectionInformation(IPEndPoint local, IPEndPoint remote, TcpState state)
		{
			this.localEndPoint = local;
			this.remoteEndPoint = remote;
			this.state = state;
		}

		public override TcpState State
		{
			get
			{
				return this.state;
			}
		}

		public override IPEndPoint LocalEndPoint
		{
			get
			{
				return this.localEndPoint;
			}
		}

		public override IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.remoteEndPoint;
			}
		}

		private IPEndPoint localEndPoint;

		private IPEndPoint remoteEndPoint;

		private TcpState state;
	}
}
