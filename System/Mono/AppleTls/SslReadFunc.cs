using System;

namespace Mono.AppleTls
{
	internal delegate SslStatus SslReadFunc(IntPtr connection, IntPtr data, ref IntPtr dataLength);
}
