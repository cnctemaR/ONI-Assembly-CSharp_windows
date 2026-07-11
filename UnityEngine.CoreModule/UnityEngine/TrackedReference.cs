using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class TrackedReference
	{
		protected TrackedReference()
		{
		}

		public static bool operator ==(TrackedReference x, TrackedReference y)
		{
			bool flag;
			if (y == null && x == null)
			{
				flag = true;
			}
			else if (y == null)
			{
				flag = x.m_Ptr == IntPtr.Zero;
			}
			else if (x == null)
			{
				flag = y.m_Ptr == IntPtr.Zero;
			}
			else
			{
				flag = x.m_Ptr == y.m_Ptr;
			}
			return flag;
		}

		public static bool operator !=(TrackedReference x, TrackedReference y)
		{
			return !(x == y);
		}

		public override bool Equals(object o)
		{
			return o as TrackedReference == this;
		}

		public override int GetHashCode()
		{
			return (int)this.m_Ptr;
		}

		public static implicit operator bool(TrackedReference exists)
		{
			return exists != null;
		}

		internal IntPtr m_Ptr;
	}
}
