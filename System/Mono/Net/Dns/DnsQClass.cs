using System;

namespace Mono.Net.Dns
{
	internal enum DnsQClass : ushort
	{
		Internet = 1,
		IN = 1,
		CSNET,
		CS = 2,
		CHAOS,
		CH = 3,
		Hesiod,
		HS = 4,
		None = 254,
		Any
	}
}
