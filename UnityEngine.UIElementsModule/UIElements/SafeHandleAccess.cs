using System;

namespace UnityEngine.UIElements
{
	internal struct SafeHandleAccess
	{
		public SafeHandleAccess(IntPtr ptr)
		{
			this.m_Handle = ptr;
		}

		public bool IsNull()
		{
			return this.m_Handle == IntPtr.Zero;
		}

		public static implicit operator IntPtr(SafeHandleAccess a)
		{
			bool flag = a.m_Handle == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentNullException();
			}
			return a.m_Handle;
		}

		private IntPtr m_Handle;
	}
}
