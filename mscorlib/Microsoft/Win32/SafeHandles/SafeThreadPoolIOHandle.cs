using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal class SafeThreadPoolIOHandle : SafeHandle
	{
		static SafeThreadPoolIOHandle()
		{
			if (!Environment.IsRunningOnWindows)
			{
				throw new PlatformNotSupportedException();
			}
		}

		private SafeThreadPoolIOHandle()
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
			Interop.mincore.CloseThreadpoolIo(this.handle);
			return true;
		}
	}
}
