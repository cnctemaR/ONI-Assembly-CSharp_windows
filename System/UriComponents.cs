using System;

namespace System
{
	[Flags]
	public enum UriComponents
	{
		Scheme = 1,
		UserInfo = 2,
		Host = 4,
		Port = 8,
		Path = 16,
		Query = 32,
		Fragment = 64,
		StrongPort = 128,
		KeepDelimiter = 1073741824,
		HostAndPort = 132,
		StrongAuthority = 134,
		AbsoluteUri = 127,
		PathAndQuery = 48,
		HttpRequestUrl = 61,
		SchemeAndServer = 13,
		SerializationInfoString = -2147483648
	}
}
