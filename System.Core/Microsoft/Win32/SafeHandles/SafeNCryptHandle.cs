using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class SafeNCryptHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		protected SafeNCryptHandle()
			: base(true)
		{
		}

		protected SafeNCryptHandle(IntPtr handle, SafeHandle parentHandle)
			: base(false)
		{
			throw new NotImplementedException();
		}

		public override bool IsInvalid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override bool ReleaseHandle()
		{
			return false;
		}

		protected abstract bool ReleaseNativeHandle();
	}
}
