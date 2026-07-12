using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafePasswordHandle : SafeHandle
	{
		private IntPtr CreateHandle(string password)
		{
			return Marshal.StringToHGlobalUni(password);
		}

		private IntPtr CreateHandle(SecureString password)
		{
			return Marshal.SecureStringToGlobalAllocUnicode(password);
		}

		private void FreeHandle()
		{
			Marshal.ZeroFreeGlobalAllocUnicode(this.handle);
		}

		public SafePasswordHandle(string password)
			: base(IntPtr.Zero, true)
		{
			if (password != null)
			{
				base.SetHandle(this.CreateHandle(password));
			}
		}

		public SafePasswordHandle(SecureString password)
			: base(IntPtr.Zero, true)
		{
			if (password != null)
			{
				base.SetHandle(this.CreateHandle(password));
			}
		}

		protected override bool ReleaseHandle()
		{
			if (this.handle != IntPtr.Zero)
			{
				this.FreeHandle();
			}
			base.SetHandle((IntPtr)(-1));
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && SafeHandleCache<SafePasswordHandle>.IsCachedInvalidHandle(this))
			{
				return;
			}
			base.Dispose(disposing);
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == (IntPtr)(-1);
			}
		}

		public static SafePasswordHandle InvalidHandle
		{
			get
			{
				return SafeHandleCache<SafePasswordHandle>.GetInvalidHandle(() => new SafePasswordHandle(null)
				{
					handle = (IntPtr)(-1)
				});
			}
		}

		internal string Mono_DangerousGetString()
		{
			return Marshal.PtrToStringUni(base.DangerousGetHandle());
		}
	}
}
