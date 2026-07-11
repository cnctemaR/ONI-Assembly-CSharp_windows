using System;

namespace System.Net
{
	internal enum CookieVariant
	{
		Unknown,
		Plain,
		Rfc2109,
		Rfc2965,
		Default = 2
	}
}
