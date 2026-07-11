using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeGssNameHandle : SafeHandle
	{
		public static SafeGssNameHandle CreateUser(string name)
		{
			Interop.NetSecurityNative.Status status2;
			SafeGssNameHandle safeGssNameHandle;
			Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ImportUserName(out status2, name, Encoding.UTF8.GetByteCount(name), out safeGssNameHandle);
			if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
			{
				safeGssNameHandle.Dispose();
				throw new Interop.NetSecurityNative.GssApiException(status, status2);
			}
			return safeGssNameHandle;
		}

		public static SafeGssNameHandle CreatePrincipal(string name)
		{
			Interop.NetSecurityNative.Status status2;
			SafeGssNameHandle safeGssNameHandle;
			Interop.NetSecurityNative.Status status = Interop.NetSecurityNative.ImportPrincipalName(out status2, name, Encoding.UTF8.GetByteCount(name), out safeGssNameHandle);
			if (status != Interop.NetSecurityNative.Status.GSS_S_COMPLETE)
			{
				safeGssNameHandle.Dispose();
				throw new Interop.NetSecurityNative.GssApiException(status, status2);
			}
			return safeGssNameHandle;
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
			int num = (int)Interop.NetSecurityNative.ReleaseName(out status, ref this.handle);
			base.SetHandle(IntPtr.Zero);
			return num == 0;
		}

		private SafeGssNameHandle()
			: base(IntPtr.Zero, true)
		{
		}
	}
}
