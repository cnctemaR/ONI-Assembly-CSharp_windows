using System;

namespace Mono.Net.Dns
{
	internal class DnsResourceRecordPTR : DnsResourceRecord
	{
		internal DnsResourceRecordPTR(DnsResourceRecord rr)
		{
			base.CopyFrom(rr);
			int offset = rr.Data.Offset;
			this.dname = DnsPacket.ReadName(rr.Data.Array, ref offset);
		}

		public string DName
		{
			get
			{
				return this.dname;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " DNAME: " + this.dname.ToString();
		}

		private string dname;
	}
}
