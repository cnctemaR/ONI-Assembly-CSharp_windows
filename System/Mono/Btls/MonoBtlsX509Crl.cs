using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Mono.Btls
{
	internal class MonoBtlsX509Crl : MonoBtlsObject
	{
		internal new MonoBtlsX509Crl.BoringX509CrlHandle Handle
		{
			get
			{
				return (MonoBtlsX509Crl.BoringX509CrlHandle)base.Handle;
			}
		}

		internal MonoBtlsX509Crl(MonoBtlsX509Crl.BoringX509CrlHandle handle)
			: base(handle)
		{
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_crl_ref(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_crl_from_data(IntPtr data, int len, MonoBtlsX509Format format);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_crl_get_by_cert(IntPtr handle, IntPtr x509);

		[DllImport("libmono-btls-shared")]
		private unsafe static extern IntPtr mono_btls_x509_crl_get_by_serial(IntPtr handle, void* serial, int len);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_crl_get_revoked_count(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_crl_get_revoked(IntPtr handle, int index);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_crl_get_last_update(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_crl_get_next_update(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern long mono_btls_x509_crl_get_version(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_crl_get_issuer(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_crl_free(IntPtr handle);

		public static MonoBtlsX509Crl LoadFromData(byte[] buffer, MonoBtlsX509Format format)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			MonoBtlsX509Crl monoBtlsX509Crl;
			try
			{
				Marshal.Copy(buffer, 0, intPtr, buffer.Length);
				IntPtr intPtr2 = MonoBtlsX509Crl.mono_btls_x509_crl_from_data(intPtr, buffer.Length, format);
				if (intPtr2 == IntPtr.Zero)
				{
					throw new MonoBtlsException("Failed to read CRL from data.");
				}
				monoBtlsX509Crl = new MonoBtlsX509Crl(new MonoBtlsX509Crl.BoringX509CrlHandle(intPtr2));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return monoBtlsX509Crl;
		}

		public MonoBtlsX509Revoked GetByCert(MonoBtlsX509 x509)
		{
			IntPtr intPtr = MonoBtlsX509Crl.mono_btls_x509_crl_get_by_cert(this.Handle.DangerousGetHandle(), x509.Handle.DangerousGetHandle());
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new MonoBtlsX509Revoked(new MonoBtlsX509Revoked.BoringX509RevokedHandle(intPtr));
		}

		public unsafe MonoBtlsX509Revoked GetBySerial(byte[] serial)
		{
			void* ptr;
			if (serial == null || serial.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = (void*)(&serial[0]);
			}
			IntPtr intPtr = MonoBtlsX509Crl.mono_btls_x509_crl_get_by_serial(this.Handle.DangerousGetHandle(), ptr, serial.Length);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new MonoBtlsX509Revoked(new MonoBtlsX509Revoked.BoringX509RevokedHandle(intPtr));
		}

		public int GetRevokedCount()
		{
			return MonoBtlsX509Crl.mono_btls_x509_crl_get_revoked_count(this.Handle.DangerousGetHandle());
		}

		public MonoBtlsX509Revoked GetRevoked(int index)
		{
			if (index >= this.GetRevokedCount())
			{
				throw new ArgumentOutOfRangeException();
			}
			IntPtr intPtr = MonoBtlsX509Crl.mono_btls_x509_crl_get_revoked(this.Handle.DangerousGetHandle(), index);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new MonoBtlsX509Revoked(new MonoBtlsX509Revoked.BoringX509RevokedHandle(intPtr));
		}

		public DateTime GetLastUpdate()
		{
			long num = MonoBtlsX509Crl.mono_btls_x509_crl_get_last_update(this.Handle.DangerousGetHandle());
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)num);
		}

		public DateTime GetNextUpdate()
		{
			long num = MonoBtlsX509Crl.mono_btls_x509_crl_get_next_update(this.Handle.DangerousGetHandle());
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)num);
		}

		public long GetVersion()
		{
			return MonoBtlsX509Crl.mono_btls_x509_crl_get_version(this.Handle.DangerousGetHandle());
		}

		public MonoBtlsX509Name GetIssuerName()
		{
			IntPtr intPtr = MonoBtlsX509Crl.mono_btls_x509_crl_get_issuer(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "GetIssuerName");
			return new MonoBtlsX509Name(new MonoBtlsX509Name.BoringX509NameHandle(intPtr, false));
		}

		internal class BoringX509CrlHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509CrlHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (this.handle != IntPtr.Zero)
				{
					MonoBtlsX509Crl.mono_btls_x509_crl_free(this.handle);
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
