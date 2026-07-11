using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime
{
	internal abstract class InternalBufferManager
	{
		public abstract byte[] TakeBuffer(int bufferSize);

		public abstract void ReturnBuffer(byte[] buffer);

		public abstract void Clear();

		public static InternalBufferManager Create(long maxBufferPoolSize, int maxBufferSize)
		{
			if (maxBufferPoolSize == 0L)
			{
				return InternalBufferManager.GCBufferManager.Value;
			}
			return new InternalBufferManager.PooledBufferManager(maxBufferPoolSize, maxBufferSize);
		}

		private class PooledBufferManager : InternalBufferManager
		{
			public PooledBufferManager(long maxMemoryToPool, int maxBufferSize)
			{
				this.tuningLock = new object();
				this.memoryLimit = maxMemoryToPool;
				this.remainingMemory = maxMemoryToPool;
				List<InternalBufferManager.PooledBufferManager.BufferPool> list = new List<InternalBufferManager.PooledBufferManager.BufferPool>();
				int num = 128;
				for (;;)
				{
					long num2 = this.remainingMemory / (long)num;
					int num3 = ((num2 > 2147483647L) ? int.MaxValue : ((int)num2));
					if (num3 > 1)
					{
						num3 = 1;
					}
					list.Add(InternalBufferManager.PooledBufferManager.BufferPool.CreatePool(num, num3));
					this.remainingMemory -= (long)num3 * (long)num;
					if (num >= maxBufferSize)
					{
						break;
					}
					long num4 = (long)num * 2L;
					if (num4 > (long)maxBufferSize)
					{
						num = maxBufferSize;
					}
					else
					{
						num = (int)num4;
					}
				}
				this.bufferPools = list.ToArray();
				this.bufferSizes = new int[this.bufferPools.Length];
				for (int i = 0; i < this.bufferPools.Length; i++)
				{
					this.bufferSizes[i] = this.bufferPools[i].BufferSize;
				}
			}

			public override void Clear()
			{
				for (int i = 0; i < this.bufferPools.Length; i++)
				{
					this.bufferPools[i].Clear();
				}
			}

			private void ChangeQuota(ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool, int delta)
			{
				if (TraceCore.BufferPoolChangeQuotaIsEnabled(Fx.Trace))
				{
					TraceCore.BufferPoolChangeQuota(Fx.Trace, bufferPool.BufferSize, delta);
				}
				InternalBufferManager.PooledBufferManager.BufferPool bufferPool2 = bufferPool;
				int num = bufferPool2.Limit + delta;
				InternalBufferManager.PooledBufferManager.BufferPool bufferPool3 = InternalBufferManager.PooledBufferManager.BufferPool.CreatePool(bufferPool2.BufferSize, num);
				for (int i = 0; i < num; i++)
				{
					byte[] array = bufferPool2.Take();
					if (array == null)
					{
						break;
					}
					bufferPool3.Return(array);
					bufferPool3.IncrementCount();
				}
				this.remainingMemory -= (long)(bufferPool2.BufferSize * delta);
				bufferPool = bufferPool3;
			}

			private void DecreaseQuota(ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool)
			{
				this.ChangeQuota(ref bufferPool, -1);
			}

			private int FindMostExcessivePool()
			{
				long num = 0L;
				int num2 = -1;
				for (int i = 0; i < this.bufferPools.Length; i++)
				{
					InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[i];
					if (bufferPool.Peak < bufferPool.Limit)
					{
						long num3 = (long)(bufferPool.Limit - bufferPool.Peak) * (long)bufferPool.BufferSize;
						if (num3 > num)
						{
							num2 = i;
							num = num3;
						}
					}
				}
				return num2;
			}

			private int FindMostStarvedPool()
			{
				long num = 0L;
				int num2 = -1;
				for (int i = 0; i < this.bufferPools.Length; i++)
				{
					InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[i];
					if (bufferPool.Peak == bufferPool.Limit)
					{
						long num3 = (long)bufferPool.Misses * (long)bufferPool.BufferSize;
						if (num3 > num)
						{
							num2 = i;
							num = num3;
						}
					}
				}
				return num2;
			}

			private InternalBufferManager.PooledBufferManager.BufferPool FindPool(int desiredBufferSize)
			{
				for (int i = 0; i < this.bufferSizes.Length; i++)
				{
					if (desiredBufferSize <= this.bufferSizes[i])
					{
						return this.bufferPools[i];
					}
				}
				return null;
			}

			private void IncreaseQuota(ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool)
			{
				this.ChangeQuota(ref bufferPool, 1);
			}

			public override void ReturnBuffer(byte[] buffer)
			{
				InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.FindPool(buffer.Length);
				if (bufferPool != null)
				{
					if (buffer.Length != bufferPool.BufferSize)
					{
						throw Fx.Exception.Argument("buffer", "Buffer Is Not Right Size For Buffer Manager");
					}
					if (bufferPool.Return(buffer))
					{
						bufferPool.IncrementCount();
					}
				}
			}

			public override byte[] TakeBuffer(int bufferSize)
			{
				InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.FindPool(bufferSize);
				byte[] array2;
				if (bufferPool != null)
				{
					byte[] array = bufferPool.Take();
					if (array != null)
					{
						bufferPool.DecrementCount();
						array2 = array;
					}
					else
					{
						if (bufferPool.Peak == bufferPool.Limit)
						{
							InternalBufferManager.PooledBufferManager.BufferPool bufferPool2 = bufferPool;
							int num = bufferPool2.Misses;
							bufferPool2.Misses = num + 1;
							num = this.totalMisses + 1;
							this.totalMisses = num;
							if (num >= 8)
							{
								this.TuneQuotas();
							}
						}
						if (TraceCore.BufferPoolAllocationIsEnabled(Fx.Trace))
						{
							TraceCore.BufferPoolAllocation(Fx.Trace, bufferPool.BufferSize);
						}
						array2 = Fx.AllocateByteArray(bufferPool.BufferSize);
					}
				}
				else
				{
					if (TraceCore.BufferPoolAllocationIsEnabled(Fx.Trace))
					{
						TraceCore.BufferPoolAllocation(Fx.Trace, bufferSize);
					}
					array2 = Fx.AllocateByteArray(bufferSize);
				}
				return array2;
			}

			private void TuneQuotas()
			{
				if (this.areQuotasBeingTuned)
				{
					return;
				}
				bool flag = false;
				try
				{
					Monitor.TryEnter(this.tuningLock, ref flag);
					if (!flag || this.areQuotasBeingTuned)
					{
						return;
					}
					this.areQuotasBeingTuned = true;
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this.tuningLock);
					}
				}
				int num = this.FindMostStarvedPool();
				if (num >= 0)
				{
					InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[num];
					if (this.remainingMemory < (long)bufferPool.BufferSize)
					{
						int num2 = this.FindMostExcessivePool();
						if (num2 >= 0)
						{
							this.DecreaseQuota(ref this.bufferPools[num2]);
						}
					}
					if (this.remainingMemory >= (long)bufferPool.BufferSize)
					{
						this.IncreaseQuota(ref this.bufferPools[num]);
					}
				}
				for (int i = 0; i < this.bufferPools.Length; i++)
				{
					this.bufferPools[i].Misses = 0;
				}
				this.totalMisses = 0;
				this.areQuotasBeingTuned = false;
			}

			private const int minBufferSize = 128;

			private const int maxMissesBeforeTuning = 8;

			private const int initialBufferCount = 1;

			private readonly object tuningLock;

			private int[] bufferSizes;

			private InternalBufferManager.PooledBufferManager.BufferPool[] bufferPools;

			private long memoryLimit;

			private long remainingMemory;

			private bool areQuotasBeingTuned;

			private int totalMisses;

			private abstract class BufferPool
			{
				public BufferPool(int bufferSize, int limit)
				{
					this.bufferSize = bufferSize;
					this.limit = limit;
				}

				public int BufferSize
				{
					get
					{
						return this.bufferSize;
					}
				}

				public int Limit
				{
					get
					{
						return this.limit;
					}
				}

				public int Misses
				{
					get
					{
						return this.misses;
					}
					set
					{
						this.misses = value;
					}
				}

				public int Peak
				{
					get
					{
						return this.peak;
					}
				}

				public void Clear()
				{
					this.OnClear();
					this.count = 0;
				}

				public void DecrementCount()
				{
					int num = this.count - 1;
					if (num >= 0)
					{
						this.count = num;
					}
				}

				public void IncrementCount()
				{
					int num = this.count + 1;
					if (num <= this.limit)
					{
						this.count = num;
						if (num > this.peak)
						{
							this.peak = num;
						}
					}
				}

				internal abstract byte[] Take();

				internal abstract bool Return(byte[] buffer);

				internal abstract void OnClear();

				internal static InternalBufferManager.PooledBufferManager.BufferPool CreatePool(int bufferSize, int limit)
				{
					if (bufferSize < 85000)
					{
						return new InternalBufferManager.PooledBufferManager.BufferPool.SynchronizedBufferPool(bufferSize, limit);
					}
					return new InternalBufferManager.PooledBufferManager.BufferPool.LargeBufferPool(bufferSize, limit);
				}

				private int bufferSize;

				private int count;

				private int limit;

				private int misses;

				private int peak;

				private class SynchronizedBufferPool : InternalBufferManager.PooledBufferManager.BufferPool
				{
					internal SynchronizedBufferPool(int bufferSize, int limit)
						: base(bufferSize, limit)
					{
						this.innerPool = new SynchronizedPool<byte[]>(limit);
					}

					internal override void OnClear()
					{
						this.innerPool.Clear();
					}

					internal override byte[] Take()
					{
						return this.innerPool.Take();
					}

					internal override bool Return(byte[] buffer)
					{
						return this.innerPool.Return(buffer);
					}

					private SynchronizedPool<byte[]> innerPool;
				}

				private class LargeBufferPool : InternalBufferManager.PooledBufferManager.BufferPool
				{
					internal LargeBufferPool(int bufferSize, int limit)
						: base(bufferSize, limit)
					{
						this.items = new Stack<byte[]>(limit);
					}

					private object ThisLock
					{
						get
						{
							return this.items;
						}
					}

					internal override void OnClear()
					{
						object thisLock = this.ThisLock;
						lock (thisLock)
						{
							this.items.Clear();
						}
					}

					internal override byte[] Take()
					{
						object thisLock = this.ThisLock;
						lock (thisLock)
						{
							if (this.items.Count > 0)
							{
								return this.items.Pop();
							}
						}
						return null;
					}

					internal override bool Return(byte[] buffer)
					{
						object thisLock = this.ThisLock;
						lock (thisLock)
						{
							if (this.items.Count < base.Limit)
							{
								this.items.Push(buffer);
								return true;
							}
						}
						return false;
					}

					private Stack<byte[]> items;
				}
			}
		}

		private class GCBufferManager : InternalBufferManager
		{
			private GCBufferManager()
			{
			}

			public static InternalBufferManager.GCBufferManager Value
			{
				get
				{
					return InternalBufferManager.GCBufferManager.value;
				}
			}

			public override void Clear()
			{
			}

			public override byte[] TakeBuffer(int bufferSize)
			{
				return Fx.AllocateByteArray(bufferSize);
			}

			public override void ReturnBuffer(byte[] buffer)
			{
			}

			private static InternalBufferManager.GCBufferManager value = new InternalBufferManager.GCBufferManager();
		}
	}
}
