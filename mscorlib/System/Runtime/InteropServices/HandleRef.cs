using System;

namespace System.Runtime.InteropServices
{
	public readonly struct HandleRef
	{
		public HandleRef(object wrapper, IntPtr handle)
		{
			this._wrapper = wrapper;
			this._handle = handle;
		}

		public object Wrapper
		{
			get
			{
				return this._wrapper;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return this._handle;
			}
		}

		public static explicit operator IntPtr(HandleRef value)
		{
			return value._handle;
		}

		public static IntPtr ToIntPtr(HandleRef value)
		{
			return value._handle;
		}

		private readonly object _wrapper;

		private readonly IntPtr _handle;
	}
}
