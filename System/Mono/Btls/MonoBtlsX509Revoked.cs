using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Mono.Btls
{
	internal class MonoBtlsX509Revoked : MonoBtlsObject
	{
		internal new MonoBtlsX509Revoked.BoringX509RevokedHandle Handle
		{
			get
			{
				return (MonoBtlsX509Revoked.BoringX509RevokedHandle)base.Handle;
			}
		}

		internal MonoBtlsX509Revoked(MonoBtlsX509Revoked.BoringX509RevokedHandle handle)
			: base(handle)
		{
		}

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_revoked_get_serial_number(IntPtr handle, IntPtr data, int size);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_revoked_get_revocation_date(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_revoked_get_reason(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_revoked_get_sequence(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_revoked_free(IntPtr handle);

		public byte[] GetSerialNumber()
		{
			int num = 256;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] array2;
			try
			{
				int num2 = MonoBtlsX509Revoked.mono_btls_x509_revoked_get_serial_number(this.Handle.DangerousGetHandle(), intPtr, num);
				base.CheckError(num2 > 0, "GetSerialNumber");
				byte[] array = new byte[num2];
				Marshal.Copy(intPtr, array, 0, num2);
				array2 = array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return array2;
		}

		public DateTime GetRevocationDate()
		{
			long num = MonoBtlsX509Revoked.mono_btls_x509_revoked_get_revocation_date(this.Handle.DangerousGetHandle());
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)num);
		}

		public int GetReason()
		{
			return MonoBtlsX509Revoked.mono_btls_x509_revoked_get_reason(this.Handle.DangerousGetHandle());
		}

		public int GetSequence()
		{
			return MonoBtlsX509Revoked.mono_btls_x509_revoked_get_sequence(this.Handle.DangerousGetHandle());
		}

		internal class BoringX509RevokedHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509RevokedHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (this.handle != IntPtr.Zero)
				{
					MonoBtlsX509Revoked.mono_btls_x509_revoked_free(this.handle);
				}
				return true;
			}

			public IntPtr StealHandle()
			{
				return Interlocked.Exchange(ref this.handle, IntPtr.Zero);
			}
		}
	}
}
