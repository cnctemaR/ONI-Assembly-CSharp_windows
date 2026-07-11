using System;
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
	internal class Win32UnicastIPAddressInformation : UnicastIPAddressInformation
	{
		public Win32UnicastIPAddressInformation(Win32_IP_ADAPTER_UNICAST_ADDRESS info)
		{
			this.info = info;
			IPAddress ipaddress = info.Address.GetIPAddress();
			if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
			{
				this.ipv4Mask = Win32UnicastIPAddressInformation.PrefixLengthToSubnetMask(info.OnLinkPrefixLength, ipaddress.AddressFamily);
			}
		}

		public override IPAddress Address
		{
			get
			{
				return this.info.Address.GetIPAddress();
			}
		}

		public override bool IsDnsEligible
		{
			get
			{
				return this.info.LengthFlags.IsDnsEligible;
			}
		}

		public override bool IsTransient
		{
			get
			{
				return this.info.LengthFlags.IsTransient;
			}
		}

		public override long AddressPreferredLifetime
		{
			get
			{
				return (long)((ulong)this.info.PreferredLifetime);
			}
		}

		public override long AddressValidLifetime
		{
			get
			{
				return (long)((ulong)this.info.ValidLifetime);
			}
		}

		public override long DhcpLeaseLifetime
		{
			get
			{
				return (long)((ulong)this.info.LeaseLifetime);
			}
		}

		public override DuplicateAddressDetectionState DuplicateAddressDetectionState
		{
			get
			{
				return this.info.DadState;
			}
		}

		public override IPAddress IPv4Mask
		{
			get
			{
				if (this.Address.AddressFamily != AddressFamily.InterNetwork)
				{
					return IPAddress.Any;
				}
				return this.ipv4Mask;
			}
		}

		public override PrefixOrigin PrefixOrigin
		{
			get
			{
				return this.info.PrefixOrigin;
			}
		}

		public override SuffixOrigin SuffixOrigin
		{
			get
			{
				return this.info.SuffixOrigin;
			}
		}

		private static IPAddress PrefixLengthToSubnetMask(byte prefixLength, AddressFamily family)
		{
			byte[] array;
			if (family == AddressFamily.InterNetwork)
			{
				array = new byte[4];
			}
			else
			{
				array = new byte[16];
			}
			for (int i = 0; i < (int)prefixLength; i++)
			{
				byte[] array2 = array;
				int num = i / 8;
				array2[num] |= (byte)(128 >> i % 8);
			}
			return new IPAddress(array);
		}

		private Win32_IP_ADAPTER_UNICAST_ADDRESS info;

		private IPAddress ipv4Mask;
	}
}
