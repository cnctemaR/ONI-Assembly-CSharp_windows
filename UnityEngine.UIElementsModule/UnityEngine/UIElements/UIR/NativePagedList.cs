using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class NativePagedList<T> : IDisposable where T : struct
	{
		public NativePagedList(int poolCapacity, string profilerName, Allocator firstPageAllocator = Allocator.Persistent, Allocator otherPagesAllocator = Allocator.Persistent)
		{
			Debug.Assert(poolCapacity > 0);
			this.k_PoolCapacity = Mathf.NextPowerOfTwo(poolCapacity);
			this.m_FirstPageAllocator = new NativePagedList<T>.NativeArrayAllocator(profilerName, firstPageAllocator);
			this.m_OtherPagesAllocator = new NativePagedList<T>.NativeArrayAllocator(profilerName, otherPagesAllocator);
		}

		public void Add(ref T data)
		{
			bool flag = this.m_CountInLastPage < this.m_LastPage.Length;
			if (flag)
			{
				int countInLastPage = this.m_CountInLastPage;
				this.m_CountInLastPage = countInLastPage + 1;
				this.m_LastPage[countInLastPage] = data;
			}
			else
			{
				int num = ((this.m_Pages.Count > 0) ? (this.m_LastPage.Length << 1) : this.k_PoolCapacity);
				this.m_LastPage = ((this.m_Pages.Count == 0) ? this.m_FirstPageAllocator : this.m_OtherPagesAllocator).CreateArray(num, NativeArrayOptions.UninitializedMemory);
				this.m_Pages.Add(this.m_LastPage);
				this.m_LastPage[0] = data;
				this.m_CountInLastPage = 1;
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
				bool flag2 = this.m_CountInLastPage > 0;
				if (flag2)
				{
					this.m_Enumerator.Add(this.m_LastPage.Slice<T>(0, this.m_CountInLastPage));
				}
			}
			return this.m_Enumerator;
		}

		public int GetCount()
		{
			int num = this.m_CountInLastPage;
			for (int i = 0; i < this.m_Pages.Count - 1; i++)
			{
				num += this.m_Pages[i].Length;
			}
			return num;
		}

		public void Reset()
		{
			bool flag = this.m_Pages.Count > 1;
			if (flag)
			{
				this.m_LastPage = this.m_Pages[0];
				for (int i = 1; i < this.m_Pages.Count; i++)
				{
					this.m_Pages[i].Dispose();
				}
				this.m_Pages.Clear();
				this.m_Pages.Add(this.m_LastPage);
			}
			this.m_CountInLastPage = 0;
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
					this.m_CountInLastPage = 0;
				}
				this.disposed = true;
			}
		}

		private readonly int k_PoolCapacity;

		private List<NativeArray<T>> m_Pages = new List<NativeArray<T>>(8);

		private NativeArray<T> m_LastPage;

		private int m_CountInLastPage;

		private readonly NativePagedList<T>.NativeArrayAllocator m_FirstPageAllocator;

		private readonly NativePagedList<T>.NativeArrayAllocator m_OtherPagesAllocator;

		private List<NativeSlice<T>> m_Enumerator = new List<NativeSlice<T>>(8);

		private struct NativeArrayAllocator
		{
			public NativeArrayAllocator(string profilerName, Allocator allocator)
			{
				bool flag = MemoryLabel.SupportsAllocator(allocator);
				if (flag)
				{
					this.m_Allocator = Allocator.Invalid;
					this.m_MemoryLabel = new MemoryLabel("UIElements", profilerName, allocator);
				}
				else
				{
					this.m_Allocator = allocator;
					this.m_MemoryLabel = default(MemoryLabel);
				}
			}

			public NativeArray<T> CreateArray(int length, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
			{
				bool isCreated = this.m_MemoryLabel.IsCreated;
				NativeArray<T> nativeArray;
				if (isCreated)
				{
					nativeArray = new NativeArray<T>(length, this.m_MemoryLabel, options);
				}
				else
				{
					nativeArray = new NativeArray<T>(length, this.m_Allocator, options);
				}
				return nativeArray;
			}

			private Allocator m_Allocator;

			private MemoryLabel m_MemoryLabel;
		}

		public struct Enumerator
		{
			public Enumerator(NativePagedList<T> nativePagedList, int offset)
			{
				this.m_IndexInCurrentPage = 0;
				this.m_IndexOfCurrentPage = 0;
				this.m_CountInCurrentPage = 0;
				this.m_NativePagedList = nativePagedList;
				for (int i = 0; i < this.m_NativePagedList.m_Pages.Count - 1; i++)
				{
					this.m_CountInCurrentPage = this.m_NativePagedList.m_Pages[i].Length;
					bool flag = offset >= this.m_CountInCurrentPage;
					if (!flag)
					{
						this.m_IndexInCurrentPage = offset;
						this.m_IndexOfCurrentPage = i;
						this.m_CurrentPage = this.m_NativePagedList.m_Pages[this.m_IndexOfCurrentPage];
						return;
					}
					offset -= this.m_CountInCurrentPage;
				}
				this.m_IndexOfCurrentPage = this.m_NativePagedList.m_Pages.Count - 1;
				this.m_CountInCurrentPage = this.m_NativePagedList.m_CountInLastPage;
				this.m_IndexInCurrentPage = offset;
				this.m_CurrentPage = this.m_NativePagedList.m_LastPage;
			}

			public bool HasNext()
			{
				return this.m_IndexInCurrentPage < this.m_CountInCurrentPage;
			}

			public T GetNext()
			{
				bool flag = !this.HasNext();
				if (flag)
				{
					throw new InvalidOperationException("No more elements");
				}
				T t = this.m_CurrentPage[this.m_IndexInCurrentPage];
				this.m_IndexInCurrentPage++;
				bool flag2 = this.m_IndexInCurrentPage == this.m_CountInCurrentPage;
				if (flag2)
				{
					this.m_IndexInCurrentPage = 0;
					this.m_IndexOfCurrentPage++;
					int count = this.m_NativePagedList.m_Pages.Count;
					bool flag3 = this.m_IndexOfCurrentPage < count;
					if (flag3)
					{
						bool flag4 = this.m_IndexOfCurrentPage < count - 1;
						if (flag4)
						{
							this.m_CountInCurrentPage = this.m_NativePagedList.m_Pages[this.m_IndexOfCurrentPage].Length;
						}
						else
						{
							this.m_CountInCurrentPage = this.m_NativePagedList.m_CountInLastPage;
						}
					}
					else
					{
						this.m_IndexOfCurrentPage = count - 1;
						this.m_CountInCurrentPage = this.m_NativePagedList.m_CountInLastPage;
						this.m_IndexInCurrentPage = this.m_CountInCurrentPage;
					}
					this.m_CurrentPage = this.m_NativePagedList.m_Pages[this.m_IndexOfCurrentPage];
				}
				return t;
			}

			private NativePagedList<T> m_NativePagedList;

			private NativeArray<T> m_CurrentPage;

			private int m_IndexInCurrentPage;

			private int m_IndexOfCurrentPage;

			private int m_CountInCurrentPage;
		}
	}
}
