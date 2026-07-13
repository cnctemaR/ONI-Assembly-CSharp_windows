using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class NativeList<T> : IDisposable where T : struct
	{
		public NativeList(int initialCapacity, MemoryLabel allocLabel)
		{
			Debug.Assert(initialCapacity > 0);
			this.m_MemoryLabel = allocLabel;
			this.m_NativeArray = new NativeArray<T>(initialCapacity, allocLabel, NativeArrayOptions.UninitializedMemory);
		}

		public NativeList(int initialCapacity, MemoryLabel allocLabel, Allocator allocator)
		{
			Debug.Assert(initialCapacity > 0);
			this.m_MemoryLabel = allocLabel;
			this.m_NativeArray = new NativeArray<T>(initialCapacity, allocator, NativeArrayOptions.UninitializedMemory);
		}

		private void Expand(int newLength)
		{
			NativeArray<T> nativeArray = new NativeArray<T>(newLength, this.m_MemoryLabel, NativeArrayOptions.UninitializedMemory);
			nativeArray.Slice<T>(0, this.m_Count).CopyFrom(this.m_NativeArray);
			this.m_NativeArray.Dispose();
			this.m_NativeArray = nativeArray;
		}

		public void Add(ref T data)
		{
			bool flag = this.m_Count == this.m_NativeArray.Length;
			if (flag)
			{
				this.Expand(this.m_NativeArray.Length << 1);
			}
			int count = this.m_Count;
			this.m_Count = count + 1;
			this.m_NativeArray[count] = data;
		}

		public void Add(NativeSlice<T> src)
		{
			int num = this.m_Count + src.Length;
			bool flag = this.m_NativeArray.Length < num;
			if (flag)
			{
				this.Expand(num << 1);
			}
			this.m_NativeArray.Slice<T>(this.m_Count, src.Length).CopyFrom(src);
			this.m_Count += src.Length;
		}

		public void Clear()
		{
			this.m_Count = 0;
		}

		public NativeSlice<T> GetSlice(int start, int length)
		{
			return this.m_NativeArray.Slice<T>(start, length);
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_NativeArray.Dispose();
				}
				this.disposed = true;
			}
		}

		private readonly MemoryLabel m_MemoryLabel;

		private NativeArray<T> m_NativeArray;

		private int m_Count;
	}
}
