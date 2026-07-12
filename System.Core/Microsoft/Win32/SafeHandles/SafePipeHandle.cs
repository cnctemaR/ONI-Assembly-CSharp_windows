using System;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafePipeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			return global::Interop.Kernel32.CloseHandle(this.handle);
		}

		internal SafePipeHandle()
			: this(new IntPtr(0), true)
		{
		}

		public SafePipeHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		internal void SetHandle(int descriptor)
		{
			base.SetHandle((IntPtr)descriptor);
		}

		private const int DefaultInvalidHandle = 0;
	}
}
