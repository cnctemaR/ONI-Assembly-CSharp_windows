using System;
using System.Net.Sockets;
using Mono.Unix;

namespace Mono.Remoting.Channels.Unix
{
	internal class ReusableUnixClient : UnixClient
	{
		public ReusableUnixClient(string path)
			: base(path)
		{
		}

		public bool IsAlive
		{
			get
			{
				return !base.Client.Poll(0, SelectMode.SelectRead);
			}
		}
	}
}
