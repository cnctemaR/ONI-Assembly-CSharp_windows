using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	internal class HGlobalUnmanagedMemoryStream : UnmanagedMemoryStream
	{
		public unsafe HGlobalUnmanagedMemoryStream(byte* pointer, long length, IntPtr ptr)
			: base(pointer, length, length, FileAccess.ReadWrite)
		{
			this.ptr = ptr;
		}

		protected override void Dispose(bool disposing)
		{
			if (this._isOpen)
			{
				Marshal.FreeHGlobal(this.ptr);
			}
			base.Dispose(disposing);
		}

		private IntPtr ptr;
	}
}
