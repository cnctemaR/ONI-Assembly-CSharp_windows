using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32NetworkInterface
	{
		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetNetworkParams(IntPtr ptr, ref int size);

		[DllImport("kernel32.dll", SetLastError = true)]
		private unsafe static extern int MultiByteToWideChar(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, char* lpWideCharStr, int cchWideChar);

		public unsafe static Win32_FIXED_INFO FixedInfo
		{
			get
			{
				if (!Win32NetworkInterface.initialized)
				{
					int num = 0;
					Win32NetworkInterface.GetNetworkParams(IntPtr.Zero, ref num);
					IntPtr intPtr = Marshal.AllocHGlobal(num);
					Win32NetworkInterface.GetNetworkParams(intPtr, ref num);
					Win32_FIXED_INFO_Marshal win32_FIXED_INFO_Marshal = Marshal.PtrToStructure<Win32_FIXED_INFO_Marshal>(intPtr);
					Win32NetworkInterface.fixedInfo = new Win32_FIXED_INFO
					{
						HostName = Win32NetworkInterface.<get_FixedInfo>g__GetStringFromMultiByte|5_0(&win32_FIXED_INFO_Marshal.HostName.FixedElementField),
						DomainName = Win32NetworkInterface.<get_FixedInfo>g__GetStringFromMultiByte|5_0(&win32_FIXED_INFO_Marshal.DomainName.FixedElementField),
						CurrentDnsServer = win32_FIXED_INFO_Marshal.CurrentDnsServer,
						DnsServerList = win32_FIXED_INFO_Marshal.DnsServerList,
						NodeType = win32_FIXED_INFO_Marshal.NodeType,
						ScopeId = Win32NetworkInterface.<get_FixedInfo>g__GetStringFromMultiByte|5_0(&win32_FIXED_INFO_Marshal.ScopeId.FixedElementField),
						EnableRouting = win32_FIXED_INFO_Marshal.EnableRouting,
						EnableProxy = win32_FIXED_INFO_Marshal.EnableProxy,
						EnableDns = win32_FIXED_INFO_Marshal.EnableDns
					};
					Win32NetworkInterface.initialized = true;
				}
				return Win32NetworkInterface.fixedInfo;
			}
		}

		[CompilerGenerated]
		internal unsafe static string <get_FixedInfo>g__GetStringFromMultiByte|5_0(byte* bytes)
		{
			int num = Win32NetworkInterface.MultiByteToWideChar(0U, 0U, bytes, -1, null, 0);
			if (num == 0)
			{
				return string.Empty;
			}
			char[] array2;
			char[] array = (array2 = new char[num]);
			char* ptr;
			if (array == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			Win32NetworkInterface.MultiByteToWideChar(0U, 0U, bytes, -1, ptr, num);
			array2 = null;
			return new string(array);
		}

		private static Win32_FIXED_INFO fixedInfo;

		private static bool initialized;
	}
}
