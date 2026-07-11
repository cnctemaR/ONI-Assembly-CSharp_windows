using System;

namespace Mono.Net.Dns
{
	internal class DnsResourceRecordA : DnsResourceRecordIPAddress
	{
		internal DnsResourceRecordA(DnsResourceRecord rr)
			: base(rr, 4)
		{
		}
	}
}
