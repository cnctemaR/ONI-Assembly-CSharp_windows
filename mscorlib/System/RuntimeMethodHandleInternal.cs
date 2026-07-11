using System;

namespace System
{
	internal struct RuntimeMethodHandleInternal
	{
		internal static RuntimeMethodHandleInternal EmptyHandle
		{
			get
			{
				return default(RuntimeMethodHandleInternal);
			}
		}

		internal bool IsNullHandle()
		{
			return this.m_handle.IsNull();
		}

		internal IntPtr Value
		{
			get
			{
				return this.m_handle;
			}
		}

		internal RuntimeMethodHandleInternal(IntPtr value)
		{
			this.m_handle = value;
		}

		internal IntPtr m_handle;
	}
}
