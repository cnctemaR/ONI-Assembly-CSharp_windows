using System;
using System.Runtime.InteropServices;
using Mono.Util;

namespace Mono.Btls
{
	internal abstract class MonoBtlsX509LookupMono : MonoBtlsObject
	{
		internal new MonoBtlsX509LookupMono.BoringX509LookupMonoHandle Handle
		{
			get
			{
				return (MonoBtlsX509LookupMono.BoringX509LookupMonoHandle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_lookup_mono_new();

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_lookup_mono_init(IntPtr handle, IntPtr instance, IntPtr by_subject_func);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_lookup_mono_free(IntPtr handle);

		internal MonoBtlsX509LookupMono()
			: base(new MonoBtlsX509LookupMono.BoringX509LookupMonoHandle(MonoBtlsX509LookupMono.mono_btls_x509_lookup_mono_new()))
		{
			this.gch = GCHandle.Alloc(this);
			this.instance = GCHandle.ToIntPtr(this.gch);
			this.bySubjectFunc = new MonoBtlsX509LookupMono.BySubjectFunc(MonoBtlsX509LookupMono.OnGetBySubject);
			this.bySubjectFuncPtr = Marshal.GetFunctionPointerForDelegate<MonoBtlsX509LookupMono.BySubjectFunc>(this.bySubjectFunc);
			MonoBtlsX509LookupMono.mono_btls_x509_lookup_mono_init(this.Handle.DangerousGetHandle(), this.instance, this.bySubjectFuncPtr);
		}

		internal void Install(MonoBtlsX509Lookup lookup)
		{
			if (this.lookup != null)
			{
				throw new InvalidOperationException();
			}
			this.lookup = lookup;
		}

		protected void AddCertificate(MonoBtlsX509 certificate)
		{
			this.lookup.AddCertificate(certificate);
		}

		protected abstract MonoBtlsX509 OnGetBySubject(MonoBtlsX509Name name);

		[MonoPInvokeCallback(typeof(MonoBtlsX509LookupMono.BySubjectFunc))]
		private static int OnGetBySubject(IntPtr instance, IntPtr name_ptr, out IntPtr x509_ptr)
		{
			int num;
			try
			{
				MonoBtlsX509Name.BoringX509NameHandle boringX509NameHandle = null;
				try
				{
					MonoBtlsX509LookupMono monoBtlsX509LookupMono = (MonoBtlsX509LookupMono)GCHandle.FromIntPtr(instance).Target;
					boringX509NameHandle = new MonoBtlsX509Name.BoringX509NameHandle(name_ptr, false);
					MonoBtlsX509Name monoBtlsX509Name = new MonoBtlsX509Name(boringX509NameHandle);
					MonoBtlsX509 monoBtlsX = monoBtlsX509LookupMono.OnGetBySubject(monoBtlsX509Name);
					if (monoBtlsX != null)
					{
						x509_ptr = monoBtlsX.Handle.StealHandle();
						num = 1;
					}
					else
					{
						x509_ptr = IntPtr.Zero;
						num = 0;
					}
				}
				finally
				{
					if (boringX509NameHandle != null)
					{
						boringX509NameHandle.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("LOOKUP METHOD - GET BY SUBJECT EX: {0}", ex);
				x509_ptr = IntPtr.Zero;
				num = 0;
			}
			return num;
		}

		protected override void Close()
		{
			try
			{
				if (this.gch.IsAllocated)
				{
					this.gch.Free();
				}
			}
			finally
			{
				this.instance = IntPtr.Zero;
				this.bySubjectFunc = null;
				this.bySubjectFuncPtr = IntPtr.Zero;
				base.Close();
			}
		}

		private GCHandle gch;

		private IntPtr instance;

		private MonoBtlsX509LookupMono.BySubjectFunc bySubjectFunc;

		private IntPtr bySubjectFuncPtr;

		private MonoBtlsX509Lookup lookup;

		internal class BoringX509LookupMonoHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509LookupMonoHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsX509LookupMono.mono_btls_x509_lookup_mono_free(this.handle);
				return true;
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int BySubjectFunc(IntPtr instance, IntPtr name, out IntPtr x509_ptr);
	}
}
