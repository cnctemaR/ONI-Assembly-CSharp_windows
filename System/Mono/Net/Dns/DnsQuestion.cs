using System;

namespace Mono.Net.Dns
{
	internal class DnsQuestion
	{
		internal DnsQuestion()
		{
		}

		internal int Init(DnsPacket packet, int offset)
		{
			this.name = packet.ReadName(ref offset);
			this.type = (DnsQType)packet.ReadUInt16(ref offset);
			this._class = (DnsQClass)packet.ReadUInt16(ref offset);
			return offset;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public DnsQType Type
		{
			get
			{
				return this.type;
			}
		}

		public DnsQClass Class
		{
			get
			{
				return this._class;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1} Class: {2}", this.Name, this.Type, this.Class);
		}

		private string name;

		private DnsQType type;

		private DnsQClass _class;
	}
}
