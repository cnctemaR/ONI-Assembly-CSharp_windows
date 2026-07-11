using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32
{
	[SuppressUnmanagedCodeSecurity]
	internal static class UnsafeNativeMethods
	{
		[SecurityCritical]
		[DllImport("kernel32.dll", EntryPoint = "GetCurrentPackageId")]
		[return: MarshalAs(UnmanagedType.I4)]
		private static extern int _GetCurrentPackageId(ref int pBufferLength, byte[] pBuffer);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string methodName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[SecurityCritical]
		private static bool DoesWin32MethodExist(string moduleName, string methodName)
		{
			IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle(moduleName);
			return !(moduleHandle == IntPtr.Zero) && UnsafeNativeMethods.GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
		}

		[SecuritySafeCritical]
		private static bool _IsPackagedProcess()
		{
			OperatingSystem osversion = Environment.OSVersion;
			if (osversion.Platform == PlatformID.Win32NT && osversion.Version >= new Version(6, 2, 0, 0) && UnsafeNativeMethods.DoesWin32MethodExist("kernel32.dll", "GetCurrentPackageId"))
			{
				int num = 0;
				return UnsafeNativeMethods._GetCurrentPackageId(ref num, null) == 122;
			}
			return false;
		}

		internal const string KERNEL32 = "kernel32.dll";

		internal const int ERROR_INSUFFICIENT_BUFFER = 122;

		internal const int ERROR_NO_PACKAGE_IDENTITY = 15700;

		[SecuritySafeCritical]
		internal static Lazy<bool> IsPackagedProcess = new Lazy<bool>(() => UnsafeNativeMethods._IsPackagedProcess());
	}
}
