using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Mono.Btls
{
	internal class MonoBtlsPkcs12 : MonoBtlsObject
	{
		internal new MonoBtlsPkcs12.BoringPkcs12Handle Handle
		{
			get
			{
				return (MonoBtlsPkcs12.BoringPkcs12Handle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_pkcs12_free(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_pkcs12_new();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_pkcs12_get_count(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_pkcs12_get_cert(IntPtr Handle, int index);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_pkcs12_add_cert(IntPtr chain, IntPtr x509);

		[DllImport("libmono-btls-shared")]
		private unsafe static extern int mono_btls_pkcs12_import(IntPtr chain, void* data, int len, SafePasswordHandle password);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_pkcs12_has_private_key(IntPtr pkcs12);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_pkcs12_get_private_key(IntPtr pkcs12);

		internal MonoBtlsPkcs12()
			: base(new MonoBtlsPkcs12.BoringPkcs12Handle(MonoBtlsPkcs12.mono_btls_pkcs12_new()))
		{
		}

		internal MonoBtlsPkcs12(MonoBtlsPkcs12.BoringPkcs12Handle handle)
			: base(handle)
		{
		}

		public int Count
		{
			get
			{
				return MonoBtlsPkcs12.mono_btls_pkcs12_get_count(this.Handle.DangerousGetHandle());
			}
		}

		public MonoBtlsX509 GetCertificate(int index)
		{
			if (index >= this.Count)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr intPtr = MonoBtlsPkcs12.mono_btls_pkcs12_get_cert(this.Handle.DangerousGetHandle(), index);
			base.CheckError(intPtr != IntPtr.Zero, "GetCertificate");
			return new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr));
		}

		public void AddCertificate(MonoBtlsX509 x509)
		{
			MonoBtlsPkcs12.mono_btls_pkcs12_add_cert(this.Handle.DangerousGetHandle(), x509.Handle.DangerousGetHandle());
		}

		public unsafe void Import(byte[] buffer, SafePasswordHandle password)
		{
			fixed (byte[] array = buffer)
			{
				void* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = (void*)(&array[0]);
				}
				int num = MonoBtlsPkcs12.mono_btls_pkcs12_import(this.Handle.DangerousGetHandle(), ptr, buffer.Length, password);
				base.CheckError(num, "Import");
			}
		}

		public bool HasPrivateKey
		{
			get
			{
				return MonoBtlsPkcs12.mono_btls_pkcs12_has_private_key(this.Handle.DangerousGetHandle()) != 0;
			}
		}

		public MonoBtlsKey GetPrivateKey()
		{
			if (!this.HasPrivateKey)
			{
				throw new InvalidOperationException();
			}
			if (this.privateKey == null)
			{
				IntPtr intPtr = MonoBtlsPkcs12.mono_btls_pkcs12_get_private_key(this.Handle.DangerousGetHandle());
				base.CheckError(intPtr != IntPtr.Zero, "GetPrivateKey");
				this.privateKey = new MonoBtlsKey(new MonoBtlsKey.BoringKeyHandle(intPtr));
			}
			return this.privateKey;
		}

		private MonoBtlsKey privateKey;

		internal class BoringPkcs12Handle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringPkcs12Handle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsPkcs12.mono_btls_pkcs12_free(this.handle);
				return true;
			}
		}
	}
}
