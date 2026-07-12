using System;

namespace System.Net.Sockets
{
	internal sealed class DualSocketMultipleConnectAsync : MultipleConnectAsync
	{
		public DualSocketMultipleConnectAsync(SocketType socketType, ProtocolType protocolType)
		{
			if (Socket.OSSupportsIPv4)
			{
				this._socket4 = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
			}
			if (Socket.OSSupportsIPv6)
			{
				this._socket6 = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
			}
		}

		protected override IPAddress GetNextAddress(out Socket attemptSocket)
		{
			IPAddress ipaddress = null;
			attemptSocket = null;
			while (attemptSocket == null)
			{
				if (this._nextAddress >= this._addressList.Length)
				{
					return null;
				}
				ipaddress = this._addressList[this._nextAddress];
				this._nextAddress++;
				if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					attemptSocket = this._socket6;
				}
				else if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					attemptSocket = this._socket4;
				}
			}
			Socket socket = attemptSocket;
			if (socket != null)
			{
				socket.ReplaceHandleIfNecessaryAfterFailedConnect();
			}
			return ipaddress;
		}

		protected override void OnSucceed()
		{
			if (this._socket4 != null && !this._socket4.Connected)
			{
				this._socket4.Dispose();
			}
			if (this._socket6 != null && !this._socket6.Connected)
			{
				this._socket6.Dispose();
			}
		}

		protected override void OnFail(bool abortive)
		{
			Socket socket = this._socket4;
			if (socket != null)
			{
				socket.Dispose();
			}
			Socket socket2 = this._socket6;
			if (socket2 == null)
			{
				return;
			}
			socket2.Dispose();
		}

		private Socket _socket4;

		private Socket _socket6;
	}
}
