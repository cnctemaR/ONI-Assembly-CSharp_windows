using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public struct HandleRef
	{
		public HandleRef(object wrapper, IntPtr handle)
		{
			this.wrapper = wrapper;
			this.handle = handle;
		}

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public object Wrapper
		{
			get
			{
				return this.wrapper;
			}
		}

		public static IntPtr ToIntPtr(HandleRef value)
		{
			return value.Handle;
		}

		public static explicit operator IntPtr(HandleRef value)
		{
			return value.Handle;
		}

		private object wrapper;

		private IntPtr handle;
	}
}
