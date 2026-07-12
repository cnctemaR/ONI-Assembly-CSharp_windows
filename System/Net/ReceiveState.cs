using System;

namespace System.Net
{
	internal class ReceiveState
	{
		internal ReceiveState(CommandStream connection)
		{
			this.Connection = connection;
			this.Resp = new ResponseDescription();
			this.Buffer = new byte[1024];
			this.ValidThrough = 0;
		}

		private const int bufferSize = 1024;

		internal ResponseDescription Resp;

		internal int ValidThrough;

		internal byte[] Buffer;

		internal CommandStream Connection;
	}
}
