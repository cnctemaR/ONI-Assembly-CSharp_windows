using System;

namespace Mono.Net.Dns
{
	internal class DnsResourceRecordCName : DnsResourceRecord
	{
		internal DnsResourceRecordCName(DnsResourceRecord rr)
		{
			base.CopyFrom(rr);
			int offset = rr.Data.Offset;
			this.cname = DnsPacket.ReadName(rr.Data.Array, ref offset);
		}

		public string CName
		{
			get
			{
				return this.cname;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " CNAME: " + this.cname.ToString();
		}

		private string cname;
	}
}
