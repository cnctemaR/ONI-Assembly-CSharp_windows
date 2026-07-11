using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32IPInterfaceProperties2 : IPInterfaceProperties
	{
		public Win32IPInterfaceProperties2(Win32_IP_ADAPTER_ADDRESSES addr, Win32_MIB_IFROW mib4, Win32_MIB_IFROW mib6)
		{
			this.addr = addr;
			this.mib4 = mib4;
			this.mib6 = mib6;
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			return new Win32IPv4InterfaceProperties(this.addr, this.mib4);
		}

		public override IPv6InterfaceProperties GetIPv6Properties()
		{
			return new Win32IPv6InterfaceProperties(this.mib6);
		}

		public override IPAddressInformationCollection AnycastAddresses
		{
			get
			{
				return Win32IPInterfaceProperties2.Win32FromAnycast(this.addr.FirstAnycastAddress);
			}
		}

		private static IPAddressInformationCollection Win32FromAnycast(IntPtr ptr)
		{
			IPAddressInformationCollection ipaddressInformationCollection = new IPAddressInformationCollection();
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_ANYCAST_ADDRESS win32_IP_ADAPTER_ANYCAST_ADDRESS = (Win32_IP_ADAPTER_ANYCAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_ANYCAST_ADDRESS));
				ipaddressInformationCollection.InternalAdd(new SystemIPAddressInformation(win32_IP_ADAPTER_ANYCAST_ADDRESS.Address.GetIPAddress(), win32_IP_ADAPTER_ANYCAST_ADDRESS.LengthFlags.IsDnsEligible, win32_IP_ADAPTER_ANYCAST_ADDRESS.LengthFlags.IsTransient));
				intPtr = win32_IP_ADAPTER_ANYCAST_ADDRESS.Next;
			}
			return ipaddressInformationCollection;
		}

		public override IPAddressCollection DhcpServerAddresses
		{
			get
			{
				IPAddressCollection ipaddressCollection;
				try
				{
					ipaddressCollection = Win32IPAddressCollection.FromSocketAddress(this.addr.Dhcpv4Server);
				}
				catch (IndexOutOfRangeException)
				{
					ipaddressCollection = Win32IPAddressCollection.Empty;
				}
				return ipaddressCollection;
			}
		}

		public override IPAddressCollection DnsAddresses
		{
			get
			{
				return Win32IPAddressCollection.FromDnsServer(this.addr.FirstDnsServerAddress);
			}
		}

		public override string DnsSuffix
		{
			get
			{
				return this.addr.DnsSuffix;
			}
		}

		public override GatewayIPAddressInformationCollection GatewayAddresses
		{
			get
			{
				GatewayIPAddressInformationCollection gatewayIPAddressInformationCollection = new GatewayIPAddressInformationCollection();
				try
				{
					IntPtr intPtr = this.addr.FirstGatewayAddress;
					while (intPtr != IntPtr.Zero)
					{
						Win32_IP_ADAPTER_GATEWAY_ADDRESS win32_IP_ADAPTER_GATEWAY_ADDRESS = (Win32_IP_ADAPTER_GATEWAY_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_GATEWAY_ADDRESS));
						gatewayIPAddressInformationCollection.InternalAdd(new SystemGatewayIPAddressInformation(win32_IP_ADAPTER_GATEWAY_ADDRESS.Address.GetIPAddress()));
						intPtr = win32_IP_ADAPTER_GATEWAY_ADDRESS.Next;
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
				return gatewayIPAddressInformationCollection;
			}
		}

		public override bool IsDnsEnabled
		{
			get
			{
				return Win32NetworkInterface.FixedInfo.EnableDns > 0U;
			}
		}

		public override bool IsDynamicDnsEnabled
		{
			get
			{
				return this.addr.DdnsEnabled;
			}
		}

		public override MulticastIPAddressInformationCollection MulticastAddresses
		{
			get
			{
				return Win32IPInterfaceProperties2.Win32FromMulticast(this.addr.FirstMulticastAddress);
			}
		}

		private static MulticastIPAddressInformationCollection Win32FromMulticast(IntPtr ptr)
		{
			MulticastIPAddressInformationCollection multicastIPAddressInformationCollection = new MulticastIPAddressInformationCollection();
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_MULTICAST_ADDRESS win32_IP_ADAPTER_MULTICAST_ADDRESS = (Win32_IP_ADAPTER_MULTICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_MULTICAST_ADDRESS));
				multicastIPAddressInformationCollection.InternalAdd(new SystemMulticastIPAddressInformation(new SystemIPAddressInformation(win32_IP_ADAPTER_MULTICAST_ADDRESS.Address.GetIPAddress(), win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsDnsEligible, win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsTransient)));
				intPtr = win32_IP_ADAPTER_MULTICAST_ADDRESS.Next;
			}
			return multicastIPAddressInformationCollection;
		}

		public override UnicastIPAddressInformationCollection UnicastAddresses
		{
			get
			{
				UnicastIPAddressInformationCollection unicastIPAddressInformationCollection;
				try
				{
					unicastIPAddressInformationCollection = Win32IPInterfaceProperties2.Win32FromUnicast(this.addr.FirstUnicastAddress);
				}
				catch (IndexOutOfRangeException)
				{
					unicastIPAddressInformationCollection = new UnicastIPAddressInformationCollection();
				}
				return unicastIPAddressInformationCollection;
			}
		}

		private static UnicastIPAddressInformationCollection Win32FromUnicast(IntPtr ptr)
		{
			UnicastIPAddressInformationCollection unicastIPAddressInformationCollection = new UnicastIPAddressInformationCollection();
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				Win32_IP_ADAPTER_UNICAST_ADDRESS win32_IP_ADAPTER_UNICAST_ADDRESS = (Win32_IP_ADAPTER_UNICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_UNICAST_ADDRESS));
				unicastIPAddressInformationCollection.InternalAdd(new Win32UnicastIPAddressInformation(win32_IP_ADAPTER_UNICAST_ADDRESS));
				intPtr = win32_IP_ADAPTER_UNICAST_ADDRESS.Next;
			}
			return unicastIPAddressInformationCollection;
		}

		public override IPAddressCollection WinsServersAddresses
		{
			get
			{
				IPAddressCollection ipaddressCollection;
				try
				{
					ipaddressCollection = Win32IPAddressCollection.FromWinsServer(this.addr.FirstWinsServerAddress);
				}
				catch (IndexOutOfRangeException)
				{
					ipaddressCollection = Win32IPAddressCollection.Empty;
				}
				return ipaddressCollection;
			}
		}

		private readonly Win32_IP_ADAPTER_ADDRESSES addr;

		private readonly Win32_MIB_IFROW mib4;

		private readonly Win32_MIB_IFROW mib6;
	}
}
