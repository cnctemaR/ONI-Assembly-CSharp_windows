using System;
using System.IO.MemoryMappedFiles;
using Unity;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafeMemoryMappedFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeMemoryMappedFileHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			this.handle = preexistingHandle;
		}

		protected override bool ReleaseHandle()
		{
			MemoryMapImpl.CloseMapping(this.handle);
			this.handle = IntPtr.Zero;
			return true;
		}

		internal SafeMemoryMappedFileHandle()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
