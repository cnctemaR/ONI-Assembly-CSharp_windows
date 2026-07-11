using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class Win32_IP_ADAPTER_ADDRESSES
	{
		public bool DdnsEnabled
		{
			get
			{
				return (this.Flags & 1U) != 0U;
			}
		}

		public bool IsReceiveOnly
		{
			get
			{
				return (this.Flags & 8U) != 0U;
			}
		}

		public bool NoMulticast
		{
			get
			{
				return (this.Flags & 16U) != 0U;
			}
		}

		private const int MAX_ADAPTER_ADDRESS_LENGTH = 8;

		private const int IP_ADAPTER_DDNS_ENABLED = 1;

		private const int IP_ADAPTER_RECEIVE_ONLY = 8;

		private const int IP_ADAPTER_NO_MULTICAST = 16;

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

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public uint[] ZoneIndices;
	}
}
