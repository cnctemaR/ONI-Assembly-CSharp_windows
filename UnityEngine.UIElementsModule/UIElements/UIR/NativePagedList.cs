using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class NativePagedList<T> : IDisposable where T : struct
	{
		public NativePagedList(int poolCapacity)
		{
			Debug.Assert(poolCapacity > 0);
			this.k_PoolCapacity = Mathf.NextPowerOfTwo(poolCapacity);
		}

		public void Add(ref T data)
		{
			bool flag = this.m_CurrentPageCount < this.m_CurrentPage.Length;
			if (flag)
			{
				int currentPageCount = this.m_CurrentPageCount;
				this.m_CurrentPageCount = currentPageCount + 1;
				this.m_CurrentPage[currentPageCount] = data;
			}
			else
			{
				int num = ((this.m_Pages.Count > 0) ? (this.m_CurrentPage.Length << 1) : this.k_PoolCapacity);
				this.m_CurrentPage = new NativeArray<T>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.m_Pages.Add(this.m_CurrentPage);
				this.m_CurrentPage[0] = data;
				this.m_CurrentPageCount = 1;
			}
		}

		public void Add(T data)
		{
			this.Add(ref data);
		}

		public List<NativeSlice<T>> GetPages()
		{
			this.m_Enumerator.Clear();
			bool flag = this.m_Pages.Count > 0;
			if (flag)
			{
				int num = this.m_Pages.Count - 1;
				for (int i = 0; i < num; i++)
				{
					this.m_Enumerator.Add(this.m_Pages[i]);
				}
				bool flag2 = this.m_CurrentPageCount > 0;
				if (flag2)
				{
					this.m_Enumerator.Add(this.m_CurrentPage.Slice<T>(0, this.m_CurrentPageCount));
				}
			}
			return this.m_Enumerator;
		}

		public void Reset()
		{
			bool flag = this.m_Pages.Count > 1;
			if (flag)
			{
				this.m_CurrentPage = this.m_Pages[0];
				for (int i = 1; i < this.m_Pages.Count; i++)
				{
					this.m_Pages[i].Dispose();
				}
				this.m_Pages.Clear();
				this.m_Pages.Add(this.m_CurrentPage);
			}
			this.m_CurrentPageCount = 0;
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
					for (int i = 0; i < this.m_Pages.Count; i++)
					{
						this.m_Pages[i].Dispose();
					}
					this.m_Pages.Clear();
					this.m_CurrentPageCount = 0;
				}
				this.disposed = true;
			}
		}

		private readonly int k_PoolCapacity;

		private List<NativeArray<T>> m_Pages = new List<NativeArray<T>>(8);

		private NativeArray<T> m_CurrentPage;

		private int m_CurrentPageCount;

		private List<NativeSlice<T>> m_Enumerator = new List<NativeSlice<T>>(8);
	}
}
