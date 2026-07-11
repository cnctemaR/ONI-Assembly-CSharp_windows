using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeGssContextHandle : SafeHandle
	{
		public SafeGssContextHandle()
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
			int num = (int)Interop.NetSecurityNative.DeleteSecContext(out status, ref this.handle);
			base.SetHandle(IntPtr.Zero);
			return num == 0;
		}
	}
}
