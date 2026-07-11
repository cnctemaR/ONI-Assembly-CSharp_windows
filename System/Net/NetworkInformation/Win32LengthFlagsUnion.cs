using System;

namespace System.Net.NetworkInformation
{
	internal struct Win32LengthFlagsUnion
	{
		public bool IsDnsEligible
		{
			get
			{
				return (this.Flags & 1U) > 0U;
			}
		}

		public bool IsTransient
		{
			get
			{
				return (this.Flags & 2U) > 0U;
			}
		}

		private const int IP_ADAPTER_ADDRESS_DNS_ELIGIBLE = 1;

		private const int IP_ADAPTER_ADDRESS_TRANSIENT = 2;

		public uint Length;

		public uint Flags;
	}
}
