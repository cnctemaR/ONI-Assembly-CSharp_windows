using System;
using System.IO;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		internal SafeFindHandle()
			: base(true)
		{
		}

		internal SafeFindHandle(IntPtr preexistingHandle)
			: base(true)
		{
			base.SetHandle(preexistingHandle);
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return MonoIO.FindCloseFile(this.handle);
		}
	}
}
