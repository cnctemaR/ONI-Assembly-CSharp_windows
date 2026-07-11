using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
	internal class SafeGssCredHandle : SafeHandle
	{
		public static SafeGssCredHandle Create(string username, string password, bool isNtlmOnly)
		{
			if (string.IsNullOrEmpty(username))
			{
				return new SafeGssCredHandle();
			}
			SafeGssCredHandle safeGssCredHandle = null;
			using (SafeGssNameHandle safeGssNameHandle = SafeGssNameHandle.CreateUser(username))
			{
				Interop.NetSecurityNative.Status status2;
				Interop.NetSecurityNative.Status status;
				if (string.IsNullOrEmpty(password))
				{
					status = Interop.NetSecurityNative.InitiateCredSpNego(out status2, safeGssNameHandle, out safeGssCredHandle);
				}
				else
				{
					status = Interop.NetSecurityNative.InitiateCredWithPassword(out status2, isNtlmOnly, safeGssNameHandle, password, Encoding.UTF8.GetByteCount(password), out safeGssCredHandle);
				}
				if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
				{
					safeGssCredHandle.Dispose();
					throw new Interop.NetSecurityNative.GssApiException(status, status2);
				}
			}
			return safeGssCredHandle;
		}

		private SafeGssCredHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		protected override bool ReleaseHandle()
		{
			Interop.NetSecurityNative.Status status;
			int num = (int)Interop.NetSecurityNative.ReleaseCred(out status, ref this.handle);
			base.SetHandle(IntPtr.Zero);
			return num == 0;
		}
	}
}
