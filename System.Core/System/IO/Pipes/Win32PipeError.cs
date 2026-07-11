using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.IO.Pipes
{
	internal static class Win32PipeError
	{
		public static Exception GetException()
		{
			return Win32PipeError.GetException(Marshal.GetLastWin32Error());
		}

		public static Exception GetException(int errorCode)
		{
			if (errorCode == 5)
			{
				return new UnauthorizedAccessException();
			}
			return new Win32Exception(errorCode);
		}
	}
}
