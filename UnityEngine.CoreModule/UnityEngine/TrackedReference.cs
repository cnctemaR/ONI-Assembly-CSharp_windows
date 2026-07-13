using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
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
			bool flag = y == null && x == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = y == null;
				if (flag3)
				{
					flag2 = x.m_Ptr == IntPtr.Zero;
				}
				else
				{
					bool flag4 = x == null;
					if (flag4)
					{
						flag2 = y.m_Ptr == IntPtr.Zero;
					}
					else
					{
						flag2 = x.m_Ptr == y.m_Ptr;
					}
				}
			}
			return flag2;
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

		[VisibleToOtherModules(new string[] { "UnityEngine.AnimationModule" })]
		internal IntPtr m_Ptr;
	}
}
