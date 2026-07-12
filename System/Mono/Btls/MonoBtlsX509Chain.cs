using System;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsX509Chain : MonoBtlsObject
	{
		internal new MonoBtlsX509Chain.BoringX509ChainHandle Handle
		{
			get
			{
				return (MonoBtlsX509Chain.BoringX509ChainHandle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_chain_new();

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_chain_get_count(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_chain_get_cert(IntPtr Handle, int index);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_chain_add_cert(IntPtr chain, IntPtr x509);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_chain_up_ref(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_chain_free(IntPtr handle);

		public MonoBtlsX509Chain()
			: base(new MonoBtlsX509Chain.BoringX509ChainHandle(MonoBtlsX509Chain.mono_btls_x509_chain_new()))
		{
		}

		internal MonoBtlsX509Chain(MonoBtlsX509Chain.BoringX509ChainHandle handle)
			: base(handle)
		{
		}

		public int Count
		{
			get
			{
				return MonoBtlsX509Chain.mono_btls_x509_chain_get_count(this.Handle.DangerousGetHandle());
			}
		}

		public MonoBtlsX509 GetCertificate(int index)
		{
			if (index >= this.Count)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr intPtr = MonoBtlsX509Chain.mono_btls_x509_chain_get_cert(this.Handle.DangerousGetHandle(), index);
			base.CheckError(intPtr != IntPtr.Zero, "GetCertificate");
			return new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr));
		}

		public void Dump()
		{
			Console.Error.WriteLine("CHAIN: {0:x} {1}", this.Handle, this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				using (MonoBtlsX509 certificate = this.GetCertificate(i))
				{
					Console.Error.WriteLine("  CERT #{0}: {1}", i, certificate.GetSubjectNameString());
				}
			}
		}

		public void AddCertificate(MonoBtlsX509 x509)
		{
			MonoBtlsX509Chain.mono_btls_x509_chain_add_cert(this.Handle.DangerousGetHandle(), x509.Handle.DangerousGetHandle());
		}

		internal MonoBtlsX509Chain Copy()
		{
			IntPtr intPtr = MonoBtlsX509Chain.mono_btls_x509_chain_up_ref(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "Copy");
			return new MonoBtlsX509Chain(new MonoBtlsX509Chain.BoringX509ChainHandle(intPtr));
		}

		internal class BoringX509ChainHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509ChainHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsX509Chain.mono_btls_x509_chain_free(this.handle);
				return true;
			}
		}
	}
}
