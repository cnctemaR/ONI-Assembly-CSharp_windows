using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class TempAllocator<T> : IDisposable where T : struct
	{
		public TempAllocator(int poolCapacity, int excessMinCapacity, int excessMaxCapacity)
		{
			Debug.Assert(poolCapacity >= 1);
			Debug.Assert(excessMinCapacity >= 1);
			Debug.Assert(excessMinCapacity <= excessMaxCapacity);
			this.m_ExcessMinCapacity = excessMinCapacity;
			this.m_ExcessMaxCapacity = excessMaxCapacity;
			this.m_NextExcessSize = this.m_ExcessMinCapacity;
			this.m_Pool = default(TempAllocator<T>.Page);
			this.m_Pool.array = new NativeArray<T>(poolCapacity, TempAllocator<T>.k_MemoryLabel, NativeArrayOptions.UninitializedMemory);
			this.m_Excess = new List<TempAllocator<T>.Page>(8);
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
					this.Reset();
					this.m_Pool.array.Dispose();
					this.m_Pool.used = 0;
				}
				this.disposed = true;
			}
		}

		public NativeSlice<T> Alloc(int count)
		{
			bool flag = count > 0;
			NativeSlice<T> nativeSlice2;
			if (flag)
			{
				NativeSlice<T> nativeSlice = this.DoAlloc(count);
				nativeSlice2 = nativeSlice;
			}
			else
			{
				nativeSlice2 = default(NativeSlice<T>);
			}
			return nativeSlice2;
		}

		private NativeSlice<T> DoAlloc(int count)
		{
			Debug.Assert(!this.disposed);
			int num = this.m_Pool.used + count;
			bool flag = num <= this.m_Pool.array.Length;
			NativeSlice<T> nativeSlice2;
			if (flag)
			{
				NativeSlice<T> nativeSlice = this.m_Pool.array.Slice<T>(this.m_Pool.used, count);
				this.m_Pool.used = num;
				nativeSlice2 = nativeSlice;
			}
			else
			{
				bool flag2 = count > this.m_ExcessMaxCapacity;
				if (flag2)
				{
					TempAllocator<T>.Page page = new TempAllocator<T>.Page
					{
						array = new NativeArray<T>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory),
						used = count
					};
					this.m_Excess.Add(page);
					nativeSlice2 = page.array.Slice<T>(0, count);
				}
				else
				{
					for (int i = this.m_Excess.Count - 1; i >= 0; i--)
					{
						TempAllocator<T>.Page page2 = this.m_Excess[i];
						num = page2.used + count;
						bool flag3 = num <= page2.array.Length;
						if (flag3)
						{
							NativeSlice<T> nativeSlice3 = page2.array.Slice<T>(page2.used, count);
							page2.used = num;
							this.m_Excess[i] = page2;
							return nativeSlice3;
						}
					}
					while (count > this.m_NextExcessSize)
					{
						this.m_NextExcessSize <<= 1;
					}
					TempAllocator<T>.Page page3 = new TempAllocator<T>.Page
					{
						array = new NativeArray<T>(this.m_NextExcessSize, Allocator.TempJob, NativeArrayOptions.UninitializedMemory),
						used = count
					};
					this.m_Excess.Add(page3);
					this.m_NextExcessSize = Mathf.Min(this.m_NextExcessSize << 1, this.m_ExcessMaxCapacity);
					nativeSlice2 = page3.array.Slice<T>(0, count);
				}
			}
			return nativeSlice2;
		}

		public void Reset()
		{
			this.ReleaseExcess();
			this.m_Pool.used = 0;
			this.m_NextExcessSize = this.m_ExcessMinCapacity;
		}

		private void ReleaseExcess()
		{
			foreach (TempAllocator<T>.Page page in this.m_Excess)
			{
				NativeArray<T> array = page.array;
				array.Dispose();
			}
			this.m_Excess.Clear();
		}

		public TempAllocator<T>.Statistics GatherStatistics()
		{
			TempAllocator<T>.Statistics statistics = new TempAllocator<T>.Statistics
			{
				pool = new TempAllocator<T>.PageStatistics
				{
					size = this.m_Pool.array.Length,
					used = this.m_Pool.used
				},
				excess = new TempAllocator<T>.PageStatistics[this.m_Excess.Count]
			};
			for (int i = 0; i < this.m_Excess.Count; i++)
			{
				statistics.excess[i] = new TempAllocator<T>.PageStatistics
				{
					size = this.m_Excess[i].array.Length,
					used = this.m_Excess[i].used
				};
			}
			return statistics;
		}

		private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.TempAllocator", Allocator.Persistent);

		private readonly int m_ExcessMinCapacity;

		private readonly int m_ExcessMaxCapacity;

		private TempAllocator<T>.Page m_Pool;

		private List<TempAllocator<T>.Page> m_Excess;

		private int m_NextExcessSize;

		private struct Page
		{
			public NativeArray<T> array;

			public int used;
		}

		public struct Statistics
		{
			public TempAllocator<T>.PageStatistics pool;

			public TempAllocator<T>.PageStatistics[] excess;
		}

		public struct PageStatistics
		{
			public int size;

			public int used;
		}
	}
}
