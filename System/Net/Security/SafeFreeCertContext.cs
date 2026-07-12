using System;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SafeFreeCertContext : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeFreeCertContext()
			: base(true)
		{
		}

		internal void Set(IntPtr value)
		{
			this.handle = value;
		}

		protected override bool ReleaseHandle()
		{
			global::Interop.Crypt32.CertFreeCertificateContext(this.handle);
			return true;
		}

		private const uint CRYPT_ACQUIRE_SILENT_FLAG = 64U;
	}
}
