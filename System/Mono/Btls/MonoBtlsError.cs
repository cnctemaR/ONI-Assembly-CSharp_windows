using System;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal static class MonoBtlsError
	{
		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_error_peek_error();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_error_get_error();

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_error_clear_error();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_error_peek_error_line(out IntPtr file, out int line);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_error_get_error_line(out IntPtr file, out int line);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_error_get_error_string_n(int error, IntPtr buf, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_error_get_reason(int error);

		public static int PeekError()
		{
			return MonoBtlsError.mono_btls_error_peek_error();
		}

		public static int GetError()
		{
			return MonoBtlsError.mono_btls_error_get_error();
		}

		public static void ClearError()
		{
			MonoBtlsError.mono_btls_error_clear_error();
		}

		public static string GetErrorString(int error)
		{
			int num = 1024;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			string text;
			try
			{
				MonoBtlsError.mono_btls_error_get_error_string_n(error, intPtr, num);
				text = Marshal.PtrToStringAnsi(intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return text;
		}

		public static int PeekError(out string file, out int line)
		{
			IntPtr intPtr;
			int num = MonoBtlsError.mono_btls_error_peek_error_line(out intPtr, out line);
			if (intPtr != IntPtr.Zero)
			{
				file = Marshal.PtrToStringAnsi(intPtr);
				return num;
			}
			file = null;
			return num;
		}

		public static int GetError(out string file, out int line)
		{
			IntPtr intPtr;
			int num = MonoBtlsError.mono_btls_error_get_error_line(out intPtr, out line);
			if (intPtr != IntPtr.Zero)
			{
				file = Marshal.PtrToStringAnsi(intPtr);
				return num;
			}
			file = null;
			return num;
		}

		public static int GetErrorReason(int error)
		{
			return MonoBtlsError.mono_btls_error_get_reason(error);
		}
	}
}
