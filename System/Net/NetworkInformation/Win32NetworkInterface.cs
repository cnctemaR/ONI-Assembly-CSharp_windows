using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32NetworkInterface
	{
		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetNetworkParams(IntPtr ptr, ref int size);

		public static Win32_FIXED_INFO FixedInfo
		{
			get
			{
				if (!Win32NetworkInterface.initialized)
				{
					int num = 0;
					Win32NetworkInterface.GetNetworkParams(IntPtr.Zero, ref num);
					IntPtr intPtr = Marshal.AllocHGlobal(num);
					Win32NetworkInterface.GetNetworkParams(intPtr, ref num);
					Win32NetworkInterface.fixedInfo = Marshal.PtrToStructure<Win32_FIXED_INFO>(intPtr);
					Win32NetworkInterface.initialized = true;
				}
				return Win32NetworkInterface.fixedInfo;
			}
		}

		private static Win32_FIXED_INFO fixedInfo;

		private static bool initialized;
	}
}
