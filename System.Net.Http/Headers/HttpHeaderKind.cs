using System;

namespace System.Net.Http.Headers
{
	[Flags]
	internal enum HttpHeaderKind
	{
		None = 0,
		Request = 1,
		Response = 2,
		Content = 4
	}
}
