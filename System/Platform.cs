using System;
using System.Runtime.InteropServices;

namespace System
{
	internal static class Platform
	{
		[DllImport("libc")]
		private static extern int uname(IntPtr buf);

		private static void CheckOS()
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				Platform.checkedOS = true;
				return;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(8192);
			try
			{
				if (Platform.uname(intPtr) == 0)
				{
					string text = Marshal.PtrToStringAnsi(intPtr);
					if (!(text == "Darwin"))
					{
						if (text == "FreeBSD")
						{
							Platform.isFreeBSD = true;
						}
					}
					else
					{
						Platform.isMacOS = true;
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
				Platform.checkedOS = true;
			}
		}

		public static bool IsMacOS
		{
			get
			{
				if (!Platform.checkedOS)
				{
					try
					{
						Platform.CheckOS();
					}
					catch (DllNotFoundException)
					{
						Platform.isMacOS = false;
					}
				}
				return Platform.isMacOS;
			}
		}

		public static bool IsFreeBSD
		{
			get
			{
				if (!Platform.checkedOS)
				{
					Platform.CheckOS();
				}
				return Platform.isFreeBSD;
			}
		}

		private static bool checkedOS;

		private static bool isMacOS;

		private static bool isFreeBSD;
	}
}
