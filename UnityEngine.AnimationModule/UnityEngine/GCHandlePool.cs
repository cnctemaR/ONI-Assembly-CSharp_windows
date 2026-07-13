using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal class GCHandlePool
	{
		public GCHandlePool()
		{
			this.m_handles = new GCHandle[128];
		}

		public GCHandle Alloc()
		{
			bool flag = this.m_current > 0;
			GCHandle gchandle;
			if (flag)
			{
				GCHandle[] handles = this.m_handles;
				int num = this.m_current - 1;
				this.m_current = num;
				gchandle = handles[num];
			}
			else
			{
				gchandle = GCHandle.Alloc(null);
			}
			return gchandle;
		}

		public GCHandle Alloc(object o)
		{
			bool flag = this.m_current > 0;
			GCHandle gchandle2;
			if (flag)
			{
				GCHandle[] handles = this.m_handles;
				int num = this.m_current - 1;
				this.m_current = num;
				GCHandle gchandle = handles[num];
				gchandle.Target = o;
				gchandle2 = gchandle;
			}
			else
			{
				gchandle2 = GCHandle.Alloc(o);
			}
			return gchandle2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IntPtr AllocHandleIfNotNull(object o)
		{
			bool flag = o == null;
			IntPtr intPtr;
			if (flag)
			{
				intPtr = IntPtr.Zero;
			}
			else
			{
				intPtr = (IntPtr)this.Alloc(o);
			}
			return intPtr;
		}

		public void Free(GCHandle h)
		{
			bool flag = this.m_current == this.m_handles.Length;
			if (flag)
			{
				int num = this.m_handles.Length * 2;
				GCHandle[] array = new GCHandle[num];
				Array.Copy(this.m_handles, array, this.m_handles.Length);
				this.m_handles = array;
			}
			h.Target = null;
			GCHandle[] handles = this.m_handles;
			int current = this.m_current;
			this.m_current = current + 1;
			handles[current] = h;
		}

		private GCHandle[] m_handles;

		private int m_current;
	}
}
