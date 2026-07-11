using System;
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
	internal class LinuxUnicastIPAddressInformation : UnicastIPAddressInformation
	{
		public LinuxUnicastIPAddressInformation(IPAddress address)
		{
			this.address = address;
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
				byte[] addressBytes = this.address.GetAddressBytes();
				return addressBytes[0] != 169 || addressBytes[1] != 254;
			}
		}

		[MonoTODO("Always returns false")]
		public override bool IsTransient
		{
			get
			{
				return false;
			}
		}

		public override long AddressPreferredLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long AddressValidLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long DhcpLeaseLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override DuplicateAddressDetectionState DuplicateAddressDetectionState
		{
			get
			{
				throw new NotImplementedException();
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
				if (this.ipv4Mask == null)
				{
					this.ipv4Mask = SystemNetworkInterface.GetNetMask(this.address);
				}
				return this.ipv4Mask;
			}
		}

		public override PrefixOrigin PrefixOrigin
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override SuffixOrigin SuffixOrigin
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private IPAddress address;

		private IPAddress ipv4Mask;
	}
}
