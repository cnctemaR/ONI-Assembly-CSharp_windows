using System;

namespace Mono.Net.Dns
{
	internal class DnsResourceRecordAAAA : DnsResourceRecordIPAddress
	{
		internal DnsResourceRecordAAAA(DnsResourceRecord rr)
			: base(rr, 16)
		{
		}
	}
}
