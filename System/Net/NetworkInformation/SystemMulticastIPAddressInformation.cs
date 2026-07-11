using System;

namespace System.Net.NetworkInformation
{
	internal class SystemMulticastIPAddressInformation : MulticastIPAddressInformation
	{
		private SystemMulticastIPAddressInformation()
		{
		}

		public SystemMulticastIPAddressInformation(SystemIPAddressInformation addressInfo)
		{
			this.innerInfo = addressInfo;
		}

		public override IPAddress Address
		{
			get
			{
				return this.innerInfo.Address;
			}
		}

		public override bool IsTransient
		{
			get
			{
				return this.innerInfo.IsTransient;
			}
		}

		public override bool IsDnsEligible
		{
			get
			{
				return this.innerInfo.IsDnsEligible;
			}
		}

		public override PrefixOrigin PrefixOrigin
		{
			get
			{
				return PrefixOrigin.Other;
			}
		}

		public override SuffixOrigin SuffixOrigin
		{
			get
			{
				return SuffixOrigin.Other;
			}
		}

		public override DuplicateAddressDetectionState DuplicateAddressDetectionState
		{
			get
			{
				return DuplicateAddressDetectionState.Invalid;
			}
		}

		public override long AddressValidLifetime
		{
			get
			{
				return 0L;
			}
		}

		public override long AddressPreferredLifetime
		{
			get
			{
				return 0L;
			}
		}

		public override long DhcpLeaseLifetime
		{
			get
			{
				return 0L;
			}
		}

		internal static MulticastIPAddressInformationCollection ToMulticastIpAddressInformationCollection(IPAddressInformationCollection addresses)
		{
			MulticastIPAddressInformationCollection multicastIPAddressInformationCollection = new MulticastIPAddressInformationCollection();
			foreach (IPAddressInformation ipaddressInformation in addresses)
			{
				multicastIPAddressInformationCollection.InternalAdd(new SystemMulticastIPAddressInformation((SystemIPAddressInformation)ipaddressInformation));
			}
			return multicastIPAddressInformationCollection;
		}

		private SystemIPAddressInformation innerInfo;
	}
}
