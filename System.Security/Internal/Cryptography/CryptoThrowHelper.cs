using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
	internal static class CryptoThrowHelper
	{
		public static CryptographicException ToCryptographicException(this int hr)
		{
			string message = global::Interop.Kernel32.GetMessage(hr);
			return new CryptoThrowHelper.WindowsCryptographicException(hr, message);
		}

		private sealed class WindowsCryptographicException : CryptographicException
		{
			public WindowsCryptographicException(int hr, string message)
				: base(message)
			{
				base.HResult = hr;
			}
		}
	}
}
