using System;

namespace System.Net.Sockets
{
	internal class MultipleSocketMultipleConnectAsync : MultipleConnectAsync
	{
		public MultipleSocketMultipleConnectAsync(SocketType socketType, ProtocolType protocolType)
		{
			if (Socket.OSSupportsIPv4)
			{
				this.socket4 = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
			}
			if (Socket.OSSupportsIPv6)
			{
				this.socket6 = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
			}
		}

		protected override IPAddress GetNextAddress(out Socket attemptSocket)
		{
			IPAddress ipaddress = null;
			attemptSocket = null;
			while (attemptSocket == null)
			{
				if (this.nextAddress >= this.addressList.Length)
				{
					return null;
				}
				ipaddress = this.addressList[this.nextAddress];
				this.nextAddress++;
				if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					attemptSocket = this.socket6;
				}
				else if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					attemptSocket = this.socket4;
				}
			}
			return ipaddress;
		}

		protected override void OnSucceed()
		{
			if (this.socket4 != null && !this.socket4.Connected)
			{
				this.socket4.Close();
			}
			if (this.socket6 != null && !this.socket6.Connected)
			{
				this.socket6.Close();
			}
		}

		protected override void OnFail(bool abortive)
		{
			if (this.socket4 != null)
			{
				this.socket4.Close();
			}
			if (this.socket6 != null)
			{
				this.socket6.Close();
			}
		}

		private Socket socket4;

		private Socket socket6;
	}
}
