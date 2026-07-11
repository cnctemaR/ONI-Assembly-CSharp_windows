using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct Win32_IP_ADAPTER_ADDRESSES
	{
		public bool DdnsEnabled
		{
			get
			{
				return (this.Flags & 1U) > 0U;
			}
		}

		public bool DhcpEnabled
		{
			get
			{
				return (this.Flags & 4U) > 0U;
			}
		}

		public bool IsReceiveOnly
		{
			get
			{
				return (this.Flags & 8U) > 0U;
			}
		}

		public bool NoMulticast
		{
			get
			{
				return (this.Flags & 16U) > 0U;
			}
		}

		public AlignmentUnion Alignment;

		public IntPtr Next;

		[MarshalAs(UnmanagedType.LPStr)]
		public string AdapterName;

		public IntPtr FirstUnicastAddress;

		public IntPtr FirstAnycastAddress;

		public IntPtr FirstMulticastAddress;

		public IntPtr FirstDnsServerAddress;

		public string DnsSuffix;

		public string Description;

		public string FriendlyName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] PhysicalAddress;

		public uint PhysicalAddressLength;

		public uint Flags;

		public uint Mtu;

		public NetworkInterfaceType IfType;

		public OperationalStatus OperStatus;

		public int Ipv6IfIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public uint[] ZoneIndices;

		public IntPtr FirstPrefix;

		public ulong TransmitLinkSpeed;

		public ulong ReceiveLinkSpeed;

		public IntPtr FirstWinsServerAddress;

		public IntPtr FirstGatewayAddress;

		public uint Ipv4Metric;

		public uint Ipv6Metric;

		public ulong Luid;

		public Win32_SOCKET_ADDRESS Dhcpv4Server;

		public uint CompartmentId;

		public ulong NetworkGuid;

		public int ConnectionType;

		public int TunnelType;

		public Win32_SOCKET_ADDRESS Dhcpv6Server;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
		public byte[] Dhcpv6ClientDuid;

		public ulong Dhcpv6ClientDuidLength;

		public ulong Dhcpv6Iaid;

		public IntPtr FirstDnsSuffix;

		public const int GAA_FLAG_INCLUDE_WINS_INFO = 64;

		public const int GAA_FLAG_INCLUDE_GATEWAYS = 128;

		private const int MAX_ADAPTER_ADDRESS_LENGTH = 8;

		private const int MAX_DHCPV6_DUID_LENGTH = 130;

		private const int IP_ADAPTER_DDNS_ENABLED = 1;

		private const int IP_ADAPTER_DHCP_ENABLED = 4;

		private const int IP_ADAPTER_RECEIVE_ONLY = 8;

		private const int IP_ADAPTER_NO_MULTICAST = 16;
	}
}
