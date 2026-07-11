using System;
using System.Net.Sockets;
using Mono.Posix;

namespace Mono.Unix
{
	public class PeerCred
	{
		public PeerCred(Socket sock)
		{
			if (sock.AddressFamily != AddressFamily.Unix)
			{
				throw new ArgumentException("Only Unix sockets are supported", "sock");
			}
			this.data = (PeerCredData)sock.GetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)10001);
		}

		public int ProcessID
		{
			get
			{
				return this.data.pid;
			}
		}

		public int UserID
		{
			get
			{
				return this.data.uid;
			}
		}

		public int GroupID
		{
			get
			{
				return this.data.gid;
			}
		}

		private const int so_peercred = 10001;

		private PeerCredData data;
	}
}
