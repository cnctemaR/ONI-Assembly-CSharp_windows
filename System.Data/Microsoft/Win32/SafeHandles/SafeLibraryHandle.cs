using System;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeLibraryHandle()
			: base(true)
		{
		}

		internal SafeLibraryHandle(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		protected override bool ReleaseHandle()
		{
			return global::Interop.Kernel32.FreeLibrary(this.handle);
		}
	}
}
