using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace System.Data
{
	internal static class Win32NativeMethods
	{
		internal static bool IsTokenRestrictedWrapper(IntPtr token)
		{
			bool flag;
			uint num = SNINativeMethodWrapper.UnmanagedIsTokenRestricted(token, out flag);
			if (num != 0U)
			{
				Marshal.ThrowExceptionForHR((int)num);
			}
			return flag;
		}
	}
}
