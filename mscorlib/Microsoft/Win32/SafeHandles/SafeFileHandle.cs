using System;
using System.IO;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeFileHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		internal SafeFileHandle()
			: base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			MonoIOError monoIOError;
			MonoIO.Close(this.handle, out monoIOError);
			return monoIOError == MonoIOError.ERROR_SUCCESS;
		}
	}
}
