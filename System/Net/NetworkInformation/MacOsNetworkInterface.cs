using System;

namespace System.Net.NetworkInformation
{
	internal sealed class MacOsNetworkInterface : UnixNetworkInterface
	{
		internal MacOsNetworkInterface(string name, uint ifa_flags)
			: base(name)
		{
			this._ifa_flags = ifa_flags;
		}

		public override IPInterfaceProperties GetIPProperties()
		{
			if (this.ipproperties == null)
			{
				this.ipproperties = new MacOsIPInterfaceProperties(this, this.addresses);
			}
			return this.ipproperties;
		}

		public override IPv4InterfaceStatistics GetIPv4Statistics()
		{
			if (this.ipv4stats == null)
			{
				this.ipv4stats = new MacOsIPv4InterfaceStatistics(this);
			}
			return this.ipv4stats;
		}

		public override OperationalStatus OperationalStatus
		{
			get
			{
				if ((this._ifa_flags & 1U) == 1U)
				{
					return OperationalStatus.Up;
				}
				return OperationalStatus.Unknown;
			}
		}

		public override bool SupportsMulticast
		{
			get
			{
				return (this._ifa_flags & 32768U) == 32768U;
			}
		}

		private uint _ifa_flags;
	}
}
