using System;

namespace System.Net.Sockets
{
	public struct IPPacketInformation
	{
		internal IPPacketInformation(IPAddress address, int iface)
		{
			this.address = address;
			this.iface = iface;
		}

		public IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public int Interface
		{
			get
			{
				return this.iface;
			}
		}

		public override bool Equals(object comparand)
		{
			if (!(comparand is IPPacketInformation))
			{
				return false;
			}
			IPPacketInformation ippacketInformation = (IPPacketInformation)comparand;
			return ippacketInformation.iface == this.iface && ippacketInformation.address.Equals(this.address);
		}

		public override int GetHashCode()
		{
			return this.address.GetHashCode() + this.iface;
		}

		public static bool operator ==(IPPacketInformation p1, IPPacketInformation p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(IPPacketInformation p1, IPPacketInformation p2)
		{
			return !p1.Equals(p2);
		}

		private IPAddress address;

		private int iface;
	}
}
