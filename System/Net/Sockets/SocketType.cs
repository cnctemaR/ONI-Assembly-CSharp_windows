using System;

namespace System.Net.Sockets
{
	public enum SocketType
	{
		Stream = 1,
		Dgram,
		Raw,
		Rdm,
		Seqpacket,
		Unknown = -1
	}
}
