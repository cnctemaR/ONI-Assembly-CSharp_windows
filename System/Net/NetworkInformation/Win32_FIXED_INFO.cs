using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal struct Win32_FIXED_INFO
	{
		private const int MAX_HOSTNAME_LEN = 128;

		private const int MAX_DOMAIN_NAME_LEN = 128;

		private const int MAX_SCOPE_ID_LEN = 256;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
		public string HostName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
		public string DomainName;

		public IntPtr CurrentDnsServer;

		public Win32_IP_ADDR_STRING DnsServerList;

		public NetBiosNodeType NodeType;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string ScopeId;

		public uint EnableRouting;

		public uint EnableProxy;

		public uint EnableDns;
	}
}
