using System;

namespace Mono.Net.Dns
{
	internal class DnsResourceRecord
	{
		internal DnsResourceRecord()
		{
		}

		internal void CopyFrom(DnsResourceRecord rr)
		{
			this.name = rr.name;
			this.type = rr.type;
			this.klass = rr.klass;
			this.ttl = rr.ttl;
			this.rdlength = rr.rdlength;
			this.m_rdata = rr.m_rdata;
		}

		internal static DnsResourceRecord CreateFromBuffer(DnsPacket packet, int size, ref int offset)
		{
			string text = packet.ReadName(ref offset);
			DnsType dnsType = (DnsType)packet.ReadUInt16(ref offset);
			DnsClass dnsClass = (DnsClass)packet.ReadUInt16(ref offset);
			int num = packet.ReadInt32(ref offset);
			ushort num2 = packet.ReadUInt16(ref offset);
			DnsResourceRecord dnsResourceRecord = new DnsResourceRecord();
			dnsResourceRecord.name = text;
			dnsResourceRecord.type = dnsType;
			dnsResourceRecord.klass = dnsClass;
			dnsResourceRecord.ttl = num;
			dnsResourceRecord.rdlength = num2;
			dnsResourceRecord.m_rdata = new ArraySegment<byte>(packet.Packet, offset, (int)num2);
			offset += (int)num2;
			if (dnsClass == DnsClass.Internet)
			{
				if (dnsType <= DnsType.CNAME)
				{
					if (dnsType != DnsType.A)
					{
						if (dnsType == DnsType.CNAME)
						{
							dnsResourceRecord = new DnsResourceRecordCName(dnsResourceRecord);
						}
					}
					else
					{
						dnsResourceRecord = new DnsResourceRecordA(dnsResourceRecord);
					}
				}
				else if (dnsType != DnsType.PTR)
				{
					if (dnsType == DnsType.AAAA)
					{
						dnsResourceRecord = new DnsResourceRecordAAAA(dnsResourceRecord);
					}
				}
				else
				{
					dnsResourceRecord = new DnsResourceRecordPTR(dnsResourceRecord);
				}
			}
			return dnsResourceRecord;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public DnsType Type
		{
			get
			{
				return this.type;
			}
		}

		public DnsClass Class
		{
			get
			{
				return this.klass;
			}
		}

		public int Ttl
		{
			get
			{
				return this.ttl;
			}
		}

		public ArraySegment<byte> Data
		{
			get
			{
				return this.m_rdata;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}, Class: {2}, Ttl: {3}, Data length: {4}", new object[]
			{
				this.name,
				this.type,
				this.klass,
				this.ttl,
				this.Data.Count
			});
		}

		private string name;

		private DnsType type;

		private DnsClass klass;

		private int ttl;

		private ushort rdlength;

		private ArraySegment<byte> m_rdata;
	}
}
