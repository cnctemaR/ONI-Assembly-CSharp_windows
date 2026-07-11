using System;

namespace Mono.AppleTls
{
	internal delegate SslStatus SslWriteFunc(IntPtr connection, IntPtr data, ref IntPtr dataLength);
}
