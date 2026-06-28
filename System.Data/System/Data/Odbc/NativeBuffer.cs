using System;
using System.Runtime.InteropServices;

namespace System.Data.Odbc
{
	internal sealed class NativeBuffer : IDisposable
	{
		public IntPtr Handle
		{
			get
			{
				return this._ptr;
			}
			set
			{
				this._ptr = value;
			}
		}

		public int Size
		{
			get
			{
				return this._length;
			}
		}

		public void AllocBuffer(int length)
		{
			this.FreeBuffer();
			this._ptr = Marshal.AllocCoTaskMem(length);
			this._length = length;
		}

		public void FreeBuffer()
		{
			if (this._ptr == IntPtr.Zero)
			{
				return;
			}
			Marshal.FreeCoTaskMem(this._ptr);
			this._length = 0;
			this._ptr = IntPtr.Zero;
		}

		public void EnsureAlloc(int length)
		{
			if (this.Size == length && this._ptr != IntPtr.Zero)
			{
				return;
			}
			this.AllocBuffer(length);
		}

		public void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.FreeBuffer();
			this._ptr = IntPtr.Zero;
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~NativeBuffer()
		{
			this.Dispose(false);
		}

		public static implicit operator IntPtr(NativeBuffer buf)
		{
			return buf.Handle;
		}

		private IntPtr _ptr;

		private int _length;

		private bool disposed;
	}
}
