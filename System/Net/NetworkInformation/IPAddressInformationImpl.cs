using System;

namespace System.Net.NetworkInformation
{
	internal class IPAddressInformationImpl : IPAddressInformation
	{
		public IPAddressInformationImpl(IPAddress address, bool isDnsEligible, bool isTransient)
		{
			this.address = address;
			this.is_dns_eligible = isDnsEligible;
			this.is_transient = isTransient;
		}

		public override IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public override bool IsDnsEligible
		{
			get
			{
				return this.is_dns_eligible;
			}
		}

		[global::System.MonoTODO("Always false on Linux")]
		public override bool IsTransient
		{
			get
			{
				return this.is_transient;
			}
		}

		private IPAddress address;

		private bool is_dns_eligible;

		private bool is_transient;
	}
}
