using System;

namespace Mono.Net.Dns
{
	internal enum DnsOpCode : byte
	{
		Query,
		[Obsolete]
		IQuery,
		Status,
		Notify = 4,
		Update
	}
}
