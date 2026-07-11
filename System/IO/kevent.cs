using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	internal struct kevent : IDisposable
	{
		public void Dispose()
		{
			if (this.udata != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.udata);
			}
		}

		public UIntPtr ident;

		public EventFilter filter;

		public EventFlags flags;

		public FilterFlags fflags;

		public IntPtr data;

		public IntPtr udata;
	}
}
