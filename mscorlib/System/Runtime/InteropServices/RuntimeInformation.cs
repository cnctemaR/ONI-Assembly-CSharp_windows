using System;
using System.IO;
using Mono;

namespace System.Runtime.InteropServices
{
	public static class RuntimeInformation
	{
		public static string FrameworkDescription
		{
			get
			{
				return "Mono " + Runtime.GetDisplayName();
			}
		}

		public static bool IsOSPlatform(OSPlatform osPlatform)
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if (platform == PlatformID.Win32NT)
			{
				return osPlatform == OSPlatform.Windows;
			}
			if (platform != PlatformID.Unix)
			{
				return false;
			}
			if (File.Exists("/usr/lib/libc.dylib"))
			{
				return osPlatform == OSPlatform.OSX;
			}
			return osPlatform == OSPlatform.Linux;
		}

		public static string OSDescription
		{
			get
			{
				return Environment.OSVersion.VersionString;
			}
		}

		public static Architecture OSArchitecture
		{
			get
			{
				if (!Environment.Is64BitOperatingSystem)
				{
					return Architecture.X86;
				}
				return Architecture.X64;
			}
		}

		public static Architecture ProcessArchitecture
		{
			get
			{
				if (!Environment.Is64BitProcess)
				{
					return Architecture.X86;
				}
				return Architecture.X64;
			}
		}
	}
}
