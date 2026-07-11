using System;

namespace System.Net.Sockets
{
	internal class SingleSocketMultipleConnectAsync : MultipleConnectAsync
	{
		public SingleSocketMultipleConnectAsync(Socket socket, bool userSocket)
		{
			this.socket = socket;
			this.userSocket = userSocket;
		}

		protected override IPAddress GetNextAddress(out Socket attemptSocket)
		{
			attemptSocket = this.socket;
			while (this.nextAddress < this.addressList.Length)
			{
				IPAddress ipaddress = this.addressList[this.nextAddress];
				this.nextAddress++;
				if (this.socket.CanTryAddressFamily(ipaddress.AddressFamily))
				{
					return ipaddress;
				}
			}
			return null;
		}

		protected override void OnFail(bool abortive)
		{
			if (abortive || !this.userSocket)
			{
				this.socket.Close();
			}
		}

		protected override void OnSucceed()
		{
		}

		private Socket socket;

		private bool userSocket;
	}
}
