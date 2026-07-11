using System;

namespace System.Net.NetworkInformation
{
	internal class SystemIPAddressInformation : IPAddressInformation
	{
		public SystemIPAddressInformation(IPAddress address, bool isDnsEligible, bool isTransient)
		{
			this.address = address;
			this.dnsEligible = isDnsEligible;
			this.transient = isTransient;
		}

		public override IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public override bool IsTransient
		{
			get
			{
				return this.transient;
			}
		}

		public override bool IsDnsEligible
		{
			get
			{
				return this.dnsEligible;
			}
		}

		private IPAddress address;

		internal bool transient;

		internal bool dnsEligible = true;
	}
}
