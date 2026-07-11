using System;

namespace Mono.Net.Dns
{
	internal class DnsQuery : DnsPacket
	{
		public DnsQuery(string name, DnsQType qtype, DnsQClass qclass)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int num = DnsUtil.GetEncodedLength(name);
			if (num == -1)
			{
				throw new ArgumentException("Invalid DNS name", "name");
			}
			num += 16;
			this.packet = new byte[num];
			this.header = new DnsHeader(this.packet, 0);
			this.position = 12;
			base.WriteDnsName(name);
			base.WriteUInt16((ushort)qtype);
			base.WriteUInt16((ushort)qclass);
			base.Header.QuestionCount = 1;
			base.Header.IsQuery = true;
			base.Header.RecursionDesired = true;
		}
	}
}
