using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Unity;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafeMemoryMappedViewHandle : SafeBuffer
	{
		internal SafeMemoryMappedViewHandle(IntPtr mmap_handle, IntPtr base_address, long size)
			: base(true)
		{
			this.mmap_handle = mmap_handle;
			this.handle = base_address;
			base.Initialize((ulong)size);
		}

		internal void Flush()
		{
			MemoryMapImpl.Flush(this.mmap_handle);
		}

		protected override bool ReleaseHandle()
		{
			if (this.handle != (IntPtr)(-1))
			{
				return MemoryMapImpl.Unmap(this.mmap_handle);
			}
			throw new NotImplementedException();
		}

		internal SafeMemoryMappedViewHandle()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IntPtr mmap_handle;
	}
}
