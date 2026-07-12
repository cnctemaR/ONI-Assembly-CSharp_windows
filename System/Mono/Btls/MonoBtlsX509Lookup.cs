using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsX509Lookup : MonoBtlsObject
	{
		internal new MonoBtlsX509Lookup.BoringX509LookupHandle Handle
		{
			get
			{
				return (MonoBtlsX509Lookup.BoringX509LookupHandle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_lookup_new(IntPtr store, MonoBtlsX509LookupType type);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_load_file(IntPtr handle, IntPtr file, MonoBtlsX509FileType type);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_add_dir(IntPtr handle, IntPtr dir, MonoBtlsX509FileType type);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_add_mono(IntPtr handle, IntPtr monoLookup);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_init(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_shutdown(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_lookup_by_subject(IntPtr handle, IntPtr name);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_lookup_by_fingerprint(IntPtr handle, IntPtr bytes, int len);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_lookup_free(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_lookup_peek_lookup(IntPtr handle);

		private static MonoBtlsX509Lookup.BoringX509LookupHandle Create_internal(MonoBtlsX509Store store, MonoBtlsX509LookupType type)
		{
			IntPtr intPtr = MonoBtlsX509Lookup.mono_btls_x509_lookup_new(store.Handle.DangerousGetHandle(), type);
			if (intPtr == IntPtr.Zero)
			{
				throw new MonoBtlsException();
			}
			return new MonoBtlsX509Lookup.BoringX509LookupHandle(intPtr);
		}

		internal MonoBtlsX509Lookup(MonoBtlsX509Store store, MonoBtlsX509LookupType type)
			: base(MonoBtlsX509Lookup.Create_internal(store, type))
		{
			this.store = store;
			this.type = type;
		}

		internal IntPtr GetNativeLookup()
		{
			return MonoBtlsX509Lookup.mono_btls_x509_lookup_peek_lookup(this.Handle.DangerousGetHandle());
		}

		public void LoadFile(string file, MonoBtlsX509FileType type)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				if (file != null)
				{
					intPtr = Marshal.StringToHGlobalAnsi(file);
				}
				int num = MonoBtlsX509Lookup.mono_btls_x509_lookup_load_file(this.Handle.DangerousGetHandle(), intPtr, type);
				base.CheckError(num, "LoadFile");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public void AddDirectory(string dir, MonoBtlsX509FileType type)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				if (dir != null)
				{
					intPtr = Marshal.StringToHGlobalAnsi(dir);
				}
				int num = MonoBtlsX509Lookup.mono_btls_x509_lookup_add_dir(this.Handle.DangerousGetHandle(), intPtr, type);
				base.CheckError(num, "AddDirectory");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		internal void AddMono(MonoBtlsX509LookupMono monoLookup)
		{
			if (this.type != MonoBtlsX509LookupType.MONO)
			{
				throw new NotSupportedException();
			}
			int num = MonoBtlsX509Lookup.mono_btls_x509_lookup_add_mono(this.Handle.DangerousGetHandle(), monoLookup.Handle.DangerousGetHandle());
			base.CheckError(num, "AddMono");
			monoLookup.Install(this);
			if (this.monoLookups == null)
			{
				this.monoLookups = new List<MonoBtlsX509LookupMono>();
			}
			this.monoLookups.Add(monoLookup);
		}

		public void Initialize()
		{
			int num = MonoBtlsX509Lookup.mono_btls_x509_lookup_init(this.Handle.DangerousGetHandle());
			base.CheckError(num, "Initialize");
		}

		public void Shutdown()
		{
			int num = MonoBtlsX509Lookup.mono_btls_x509_lookup_shutdown(this.Handle.DangerousGetHandle());
			base.CheckError(num, "Shutdown");
		}

		public MonoBtlsX509 LookupBySubject(MonoBtlsX509Name name)
		{
			IntPtr intPtr = MonoBtlsX509Lookup.mono_btls_x509_lookup_by_subject(this.Handle.DangerousGetHandle(), name.Handle.DangerousGetHandle());
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr));
		}

		public MonoBtlsX509 LookupByFingerPrint(byte[] fingerprint)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(fingerprint.Length);
			MonoBtlsX509 monoBtlsX;
			try
			{
				Marshal.Copy(fingerprint, 0, intPtr, fingerprint.Length);
				IntPtr intPtr2 = MonoBtlsX509Lookup.mono_btls_x509_lookup_by_fingerprint(this.Handle.DangerousGetHandle(), intPtr, fingerprint.Length);
				if (intPtr2 == IntPtr.Zero)
				{
					monoBtlsX = null;
				}
				else
				{
					monoBtlsX = new MonoBtlsX509(new MonoBtlsX509.BoringX509Handle(intPtr2));
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return monoBtlsX;
		}

		internal void AddCertificate(MonoBtlsX509 certificate)
		{
			this.store.AddCertificate(certificate);
		}

		protected override void Close()
		{
			try
			{
				if (this.monoLookups != null)
				{
					foreach (MonoBtlsX509LookupMono monoBtlsX509LookupMono in this.monoLookups)
					{
						monoBtlsX509LookupMono.Dispose();
					}
					this.monoLookups = null;
				}
			}
			finally
			{
				base.Close();
			}
		}

		private MonoBtlsX509Store store;

		private MonoBtlsX509LookupType type;

		private List<MonoBtlsX509LookupMono> monoLookups;

		internal class BoringX509LookupHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509LookupHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsX509Lookup.mono_btls_x509_lookup_free(this.handle);
				return true;
			}
		}
	}
}
