using System;
using System.Runtime.InteropServices;

namespace Mono.Btls
{
	internal class MonoBtlsX509VerifyParam : MonoBtlsObject
	{
		internal new MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle Handle
		{
			get
			{
				return (MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle)base.Handle;
			}
		}

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_verify_param_new();

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_verify_param_copy(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_verify_param_lookup(IntPtr name);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_can_modify(IntPtr param);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_name(IntPtr handle, IntPtr name);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_host(IntPtr handle, IntPtr name, int namelen);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_add_host(IntPtr handle, IntPtr name, int namelen);

		[DllImport("libmono-btls-shared")]
		private static extern ulong mono_btls_x509_verify_param_get_flags(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_flags(IntPtr handle, ulong flags);

		[DllImport("libmono-btls-shared")]
		private static extern MonoBtlsX509VerifyFlags mono_btls_x509_verify_param_get_mono_flags(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_mono_flags(IntPtr handle, MonoBtlsX509VerifyFlags flags);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_purpose(IntPtr handle, MonoBtlsX509Purpose purpose);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_get_depth(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_depth(IntPtr handle, int depth);

		[DllImport("libmono-btls-shared")]
		private static extern int mono_btls_x509_verify_param_set_time(IntPtr handle, long time);

		[DllImport("libmono-btls-shared")]
		private static extern IntPtr mono_btls_x509_verify_param_get_peername(IntPtr handle);

		[DllImport("libmono-btls-shared")]
		private static extern void mono_btls_x509_verify_param_free(IntPtr handle);

		internal MonoBtlsX509VerifyParam()
			: base(new MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle(MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_new()))
		{
		}

		internal MonoBtlsX509VerifyParam(MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle handle)
			: base(handle)
		{
		}

		public MonoBtlsX509VerifyParam Copy()
		{
			IntPtr intPtr = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_copy(this.Handle.DangerousGetHandle());
			base.CheckError(intPtr != IntPtr.Zero, "Copy");
			return new MonoBtlsX509VerifyParam(new MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle(intPtr));
		}

		public static MonoBtlsX509VerifyParam GetSslClient()
		{
			return MonoBtlsX509VerifyParam.Lookup("ssl_client", true);
		}

		public static MonoBtlsX509VerifyParam GetSslServer()
		{
			return MonoBtlsX509VerifyParam.Lookup("ssl_server", true);
		}

		public static MonoBtlsX509VerifyParam Lookup(string name, bool fail = false)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			MonoBtlsX509VerifyParam monoBtlsX509VerifyParam;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(name);
				intPtr2 = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_lookup(intPtr);
				if (intPtr2 == IntPtr.Zero)
				{
					if (fail)
					{
						throw new MonoBtlsException("X509_VERIFY_PARAM_lookup() could not find '{0}'.", new object[] { name });
					}
					monoBtlsX509VerifyParam = null;
				}
				else
				{
					monoBtlsX509VerifyParam = new MonoBtlsX509VerifyParam(new MonoBtlsX509VerifyParam.BoringX509VerifyParamHandle(intPtr2));
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return monoBtlsX509VerifyParam;
		}

		public bool CanModify
		{
			get
			{
				return MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_can_modify(this.Handle.DangerousGetHandle()) != 0;
			}
		}

		private void WantToModify()
		{
			if (!this.CanModify)
			{
				throw new MonoBtlsException("Attempting to modify read-only MonoBtlsX509VerifyParam instance.");
			}
		}

		public void SetName(string name)
		{
			this.WantToModify();
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(name);
				int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_name(this.Handle.DangerousGetHandle(), intPtr);
				base.CheckError(num, "SetName");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public void SetHost(string name)
		{
			this.WantToModify();
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(name);
				int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_host(this.Handle.DangerousGetHandle(), intPtr, name.Length);
				base.CheckError(num, "SetHost");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public void AddHost(string name)
		{
			this.WantToModify();
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(name);
				int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_add_host(this.Handle.DangerousGetHandle(), intPtr, name.Length);
				base.CheckError(num, "AddHost");
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public ulong GetFlags()
		{
			return MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_get_flags(this.Handle.DangerousGetHandle());
		}

		public void SetFlags(ulong flags)
		{
			this.WantToModify();
			int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_flags(this.Handle.DangerousGetHandle(), flags);
			base.CheckError(num, "SetFlags");
		}

		public MonoBtlsX509VerifyFlags GetMonoFlags()
		{
			return MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_get_mono_flags(this.Handle.DangerousGetHandle());
		}

		public void SetMonoFlags(MonoBtlsX509VerifyFlags flags)
		{
			this.WantToModify();
			int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_mono_flags(this.Handle.DangerousGetHandle(), flags);
			base.CheckError(num, "SetMonoFlags");
		}

		public void SetPurpose(MonoBtlsX509Purpose purpose)
		{
			this.WantToModify();
			int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_purpose(this.Handle.DangerousGetHandle(), purpose);
			base.CheckError(num, "SetPurpose");
		}

		public int GetDepth()
		{
			return MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_get_depth(this.Handle.DangerousGetHandle());
		}

		public void SetDepth(int depth)
		{
			this.WantToModify();
			int num = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_depth(this.Handle.DangerousGetHandle(), depth);
			base.CheckError(num, "SetDepth");
		}

		public void SetTime(DateTime time)
		{
			this.WantToModify();
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			long num = (long)time.Subtract(dateTime).TotalSeconds;
			int num2 = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_set_time(this.Handle.DangerousGetHandle(), num);
			base.CheckError(num2, "SetTime");
		}

		public string GetPeerName()
		{
			IntPtr intPtr = MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_get_peername(this.Handle.DangerousGetHandle());
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return Marshal.PtrToStringAnsi(intPtr);
		}

		internal class BoringX509VerifyParamHandle : MonoBtlsObject.MonoBtlsHandle
		{
			public BoringX509VerifyParamHandle(IntPtr handle)
				: base(handle, true)
			{
			}

			protected override bool ReleaseHandle()
			{
				MonoBtlsX509VerifyParam.mono_btls_x509_verify_param_free(this.handle);
				return true;
			}
		}
	}
}
