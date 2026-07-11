using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.IO
{
	internal sealed class PinnedBufferMemoryStream : UnmanagedMemoryStream
	{
		[SecurityCritical]
		private PinnedBufferMemoryStream()
		{
		}

		[SecurityCritical]
		internal unsafe PinnedBufferMemoryStream(byte[] array)
		{
			int num = array.Length;
			if (num == 0)
			{
				array = new byte[1];
				num = 0;
			}
			this._array = array;
			this._pinningHandle = new GCHandle(array, GCHandleType.Pinned);
			byte[] array2;
			byte* ptr;
			if ((array2 = this._array) == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			base.Initialize(ptr, (long)num, (long)num, FileAccess.Read, true);
			array2 = null;
		}

		~PinnedBufferMemoryStream()
		{
			this.Dispose(false);
		}

		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			if (this._isOpen)
			{
				this._pinningHandle.Free();
				this._isOpen = false;
			}
			base.Dispose(disposing);
		}

		private byte[] _array;

		private GCHandle _pinningHandle;
	}
}
