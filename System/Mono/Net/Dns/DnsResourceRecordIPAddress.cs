using System;
using System.Net;

namespace Mono.Net.Dns
{
	internal abstract class DnsResourceRecordIPAddress : DnsResourceRecord
	{
		internal DnsResourceRecordIPAddress(DnsResourceRecord rr, int address_size)
		{
			base.CopyFrom(rr);
			ArraySegment<byte> data = rr.Data;
			byte[] array = new byte[address_size];
			Buffer.BlockCopy(data.Array, data.Offset, array, 0, address_size);
			this.address = new IPAddress(array);
		}

		public override string ToString()
		{
			return base.ToString() + " Address: " + this.address;
		}

		public IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		private IPAddress address;
	}
}
