using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal struct Win32_FIXED_INFO_Marshal
	{
		private const int MAX_HOSTNAME_LEN = 128;

		private const int MAX_DOMAIN_NAME_LEN = 128;

		private const int MAX_SCOPE_ID_LEN = 256;

		[FixedBuffer(typeof(byte), 132)]
		public Win32_FIXED_INFO_Marshal.<HostName>e__FixedBuffer HostName;

		[FixedBuffer(typeof(byte), 132)]
		public Win32_FIXED_INFO_Marshal.<DomainName>e__FixedBuffer DomainName;

		public IntPtr CurrentDnsServer;

		public Win32_IP_ADDR_STRING DnsServerList;

		public NetBiosNodeType NodeType;

		[FixedBuffer(typeof(byte), 260)]
		public Win32_FIXED_INFO_Marshal.<ScopeId>e__FixedBuffer ScopeId;

		public uint EnableRouting;

		public uint EnableProxy;

		public uint EnableDns;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 132)]
		public struct <HostName>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 132)]
		public struct <DomainName>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 260)]
		public struct <ScopeId>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
