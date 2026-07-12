using System;

namespace System.Net.Sockets
{
	internal sealed class SingleSocketMultipleConnectAsync : MultipleConnectAsync
	{
		public SingleSocketMultipleConnectAsync(Socket socket, bool userSocket)
		{
			this._socket = socket;
			this._userSocket = userSocket;
		}

		protected override IPAddress GetNextAddress(out Socket attemptSocket)
		{
			this._socket.ReplaceHandleIfNecessaryAfterFailedConnect();
			while (this._nextAddress < this._addressList.Length)
			{
				IPAddress ipaddress = this._addressList[this._nextAddress];
				this._nextAddress++;
				if (this._socket.CanTryAddressFamily(ipaddress.AddressFamily))
				{
					attemptSocket = this._socket;
					return ipaddress;
				}
			}
			attemptSocket = null;
			return null;
		}

		protected override void OnFail(bool abortive)
		{
			if (abortive || !this._userSocket)
			{
				this._socket.Dispose();
			}
		}

		protected override void OnSucceed()
		{
		}

		private Socket _socket;

		private bool _userSocket;
	}
}
