using System;
using System.Security;
using Microsoft.Win32;

namespace System.IO
{
	internal static class FileSystemEnumerableHelpers
	{
		[SecurityCritical]
		internal static bool IsDir(Win32Native.WIN32_FIND_DATA data)
		{
			return (data.dwFileAttributes & 16) != 0 && !data.cFileName.Equals(".") && !data.cFileName.Equals("..");
		}

		[SecurityCritical]
		internal static bool IsFile(Win32Native.WIN32_FIND_DATA data)
		{
			return (data.dwFileAttributes & 16) == 0;
		}
	}
}
