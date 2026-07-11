using System;
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
			switch (Environment.Platform)
			{
			case PlatformID.Win32NT:
				return osPlatform == OSPlatform.Windows;
			case PlatformID.Unix:
				return osPlatform == OSPlatform.Linux;
			case PlatformID.MacOSX:
				return osPlatform == OSPlatform.OSX;
			}
			return false;
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
