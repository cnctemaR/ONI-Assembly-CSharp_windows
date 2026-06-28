using System;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	internal class ErrorMarshal
	{
		static ErrorMarshal()
		{
			try
			{
				ErrorMarshal.Translate = new ErrorMarshal.ErrorTranslator(ErrorMarshal.strerror_r);
				ErrorMarshal.Translate(Errno.ERANGE);
			}
			catch (EntryPointNotFoundException)
			{
				ErrorMarshal.Translate = new ErrorMarshal.ErrorTranslator(ErrorMarshal.strerror);
			}
		}

		private static string strerror(Errno errno)
		{
			return Stdlib.strerror(errno);
		}

		private static string strerror_r(Errno errno)
		{
			StringBuilder stringBuilder = new StringBuilder(16);
			int num;
			do
			{
				stringBuilder.Capacity *= 2;
				num = Syscall.strerror_r(errno, stringBuilder);
			}
			while (num == -1 && Stdlib.GetLastError() == Errno.ERANGE);
			if (num == -1)
			{
				return "** Unknown error code: " + (int)errno + "**";
			}
			return stringBuilder.ToString();
		}

		internal static readonly ErrorMarshal.ErrorTranslator Translate;

		internal delegate string ErrorTranslator(Errno errno);
	}
}
