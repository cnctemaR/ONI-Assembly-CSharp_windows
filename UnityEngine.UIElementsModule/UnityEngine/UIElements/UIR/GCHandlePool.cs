using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.UIR
{
	internal class GCHandlePool : IDisposable
	{
		public GCHandlePool(int capacity = 256, int allocBatchSize = 64)
		{
			this.m_Handles = new List<GCHandle>(capacity);
			this.m_UsedHandlesCount = 0;
			this.k_AllocBatchSize = allocBatchSize;
		}

		public GCHandle Get(object target)
		{
			bool flag = target == null;
			GCHandle gchandle;
			if (flag)
			{
				gchandle = default(GCHandle);
			}
			else
			{
				bool flag2 = this.m_UsedHandlesCount < this.m_Handles.Count;
				if (flag2)
				{
					List<GCHandle> handles = this.m_Handles;
					int usedHandlesCount = this.m_UsedHandlesCount;
					this.m_UsedHandlesCount = usedHandlesCount + 1;
					GCHandle gchandle2 = handles[usedHandlesCount];
					gchandle2.Target = target;
					gchandle = gchandle2;
				}
				else
				{
					GCHandle gchandle3 = GCHandle.Alloc(target);
					this.m_Handles.Add(gchandle3);
					this.m_UsedHandlesCount++;
					int i = 0;
					int num = this.k_AllocBatchSize - 1;
					while (i < num)
					{
						this.m_Handles.Add(GCHandle.Alloc(null));
						i++;
					}
					gchandle = gchandle3;
				}
			}
			return gchandle;
		}

		public IntPtr GetIntPtr(object target)
		{
			bool flag = target == null;
			IntPtr intPtr;
			if (flag)
			{
				intPtr = IntPtr.Zero;
			}
			else
			{
				intPtr = GCHandle.ToIntPtr(this.Get(target));
			}
			return intPtr;
		}

		public void ReturnAll()
		{
			for (int i = 0; i < this.m_UsedHandlesCount; i++)
			{
				GCHandle gchandle = this.m_Handles[i];
				gchandle.Target = null;
				this.m_Handles[i] = gchandle;
			}
			this.m_UsedHandlesCount = 0;
		}

		internal bool disposed { get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					foreach (GCHandle gchandle in this.m_Handles)
					{
						bool isAllocated = gchandle.IsAllocated;
						if (isAllocated)
						{
							gchandle.Free();
						}
					}
					this.m_Handles = null;
				}
				this.disposed = true;
			}
		}

		private List<GCHandle> m_Handles;

		private int m_UsedHandlesCount;

		private readonly int k_AllocBatchSize;
	}
}
