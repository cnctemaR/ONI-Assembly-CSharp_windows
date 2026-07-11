using System;
using System.ComponentModel;

namespace System.Net.Sockets
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
	public enum SocketClientAccessPolicyProtocol
	{
		Tcp,
		Http
	}
}
