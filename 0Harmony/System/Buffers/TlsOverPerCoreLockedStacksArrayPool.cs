using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Buffers
{
	[Nullable(new byte[] { 0, 1 })]
	internal sealed class TlsOverPerCoreLockedStacksArrayPool<[Nullable(2)] T> : ArrayPool<T>
	{
		[return: Nullable(new byte[] { 1, 0 })]
		private TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks CreatePerCoreLockedStacks(int bucketIndex)
		{
			TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks perCoreLockedStacks = new TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks();
			return Interlocked.CompareExchange<TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks>(ref this._buckets[bucketIndex], perCoreLockedStacks, null) ?? perCoreLockedStacks;
		}

		private int Id
		{
			get
			{
				return this.GetHashCode();
			}
		}

		[NullableContext(1)]
		public override T[] Rent(int minimumLength)
		{
			int num = Utilities.SelectBucketIndex(minimumLength);
			TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] array = TlsOverPerCoreLockedStacksArrayPool<T>.t_tlsBuckets;
			if (array != null && num < array.Length)
			{
				T[] array2 = array[num].Array;
				if (array2 != null)
				{
					array[num].Array = null;
					return array2;
				}
			}
			TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks[] buckets = this._buckets;
			if (num < buckets.Length)
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks perCoreLockedStacks = buckets[num];
				if (perCoreLockedStacks != null)
				{
					T[] array2 = perCoreLockedStacks.TryPop();
					if (array2 != null)
					{
						return array2;
					}
				}
				minimumLength = Utilities.GetMaxSizeForBucket(num);
			}
			else
			{
				if (minimumLength == 0)
				{
					return ArrayEx.Empty<T>();
				}
				if (minimumLength < 0)
				{
					throw new ArgumentOutOfRangeException("minimumLength");
				}
			}
			return new T[minimumLength];
		}

		[NullableContext(1)]
		public override void Return(T[] array, bool clearArray = false)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			int num = Utilities.SelectBucketIndex(array.Length);
			TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] array2 = TlsOverPerCoreLockedStacksArrayPool<T>.t_tlsBuckets ?? this.InitializeTlsBucketsAndTrimming();
			if (num < array2.Length)
			{
				if (clearArray)
				{
					Array.Clear(array, 0, array.Length);
				}
				if (array.Length != Utilities.GetMaxSizeForBucket(num))
				{
					throw new ArgumentException("Buffer not from this pool", "array");
				}
				TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] array3 = array2;
				int num2 = num;
				T[] array4 = array3[num2].Array;
				array3[num2] = new TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray(array);
				if (array4 != null)
				{
					(this._buckets[num] ?? this.CreatePerCoreLockedStacks(num)).TryPush(array4);
				}
			}
		}

		public bool Trim()
		{
			int tickCount = Environment.TickCount;
			Utilities.MemoryPressure memoryPressure = Utilities.GetMemoryPressure();
			TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks[] buckets = this._buckets;
			for (int i = 0; i < buckets.Length; i++)
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks perCoreLockedStacks = buckets[i];
				if (perCoreLockedStacks != null)
				{
					perCoreLockedStacks.Trim(tickCount, this.Id, memoryPressure, Utilities.GetMaxSizeForBucket(i));
				}
			}
			if (memoryPressure == Utilities.MemoryPressure.High)
			{
				using (IEnumerator<KeyValuePair<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object>> enumerator = this._allTlsBuckets.GetEnumerator<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object>())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object> keyValuePair = enumerator.Current;
						Array.Clear(keyValuePair.Key, 0, keyValuePair.Key.Length);
					}
					goto IL_0143;
				}
			}
			uint num;
			if (memoryPressure == Utilities.MemoryPressure.Medium)
			{
				num = 15000U;
			}
			else
			{
				num = 30000U;
			}
			uint num2 = num;
			using (IEnumerator<KeyValuePair<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object>> enumerator = this._allTlsBuckets.GetEnumerator<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object>())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object> keyValuePair2 = enumerator.Current;
					TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] key = keyValuePair2.Key;
					for (int j = 0; j < key.Length; j++)
					{
						if (key[j].Array != null)
						{
							int millisecondsTimeStamp = key[j].MillisecondsTimeStamp;
							if (millisecondsTimeStamp == 0)
							{
								key[j].MillisecondsTimeStamp = tickCount;
							}
							else if ((long)(tickCount - millisecondsTimeStamp) >= (long)((ulong)num2))
							{
								Interlocked.Exchange<T[]>(ref key[j].Array, null);
							}
						}
					}
				}
			}
			IL_0143:
			return !Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload();
		}

		[return: Nullable(new byte[] { 1, 0, 0 })]
		private TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] InitializeTlsBucketsAndTrimming()
		{
			TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] array = new TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[27];
			TlsOverPerCoreLockedStacksArrayPool<T>.t_tlsBuckets = array;
			this._allTlsBuckets.Add(array, null);
			if (Interlocked.Exchange(ref this._trimCallbackCreated, 1) == 0)
			{
				Gen2GcCallback.Register((object s) => ((TlsOverPerCoreLockedStacksArrayPool<T>)s).Trim(), this);
			}
			return array;
		}

		private const int NumBuckets = 27;

		private const int MaxPerCorePerArraySizeStacks = 64;

		private const int MaxBuffersPerArraySizePerCore = 8;

		[Nullable(new byte[] { 2, 0, 0 })]
		[ThreadStatic]
		private static TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[] t_tlsBuckets;

		[Nullable(new byte[] { 1, 1, 0, 0, 2 })]
		private readonly ConditionalWeakTable<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object> _allTlsBuckets = new ConditionalWeakTable<TlsOverPerCoreLockedStacksArrayPool<T>.ThreadLocalArray[], object>();

		[Nullable(new byte[] { 1, 2, 0 })]
		private readonly TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks[] _buckets = new TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks[27];

		private int _trimCallbackCreated;

		private sealed class PerCoreLockedStacks
		{
			public PerCoreLockedStacks()
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[] array = new TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks.s_lockedStackCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack();
				}
				this._perCoreStacks = array;
			}

			[NullableContext(1)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryPush(T[] array)
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[] perCoreStacks = this._perCoreStacks;
				int num = EnvironmentEx.CurrentManagedThreadId % TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks.s_lockedStackCount;
				for (int i = 0; i < perCoreStacks.Length; i++)
				{
					if (perCoreStacks[num].TryPush(array))
					{
						return true;
					}
					if (++num == perCoreStacks.Length)
					{
						num = 0;
					}
				}
				return false;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: Nullable(new byte[] { 2, 1 })]
			public T[] TryPop()
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[] perCoreStacks = this._perCoreStacks;
				int num = EnvironmentEx.CurrentManagedThreadId % TlsOverPerCoreLockedStacksArrayPool<T>.PerCoreLockedStacks.s_lockedStackCount;
				for (int i = 0; i < perCoreStacks.Length; i++)
				{
					T[] array;
					if ((array = perCoreStacks[num].TryPop()) != null)
					{
						return array;
					}
					if (++num == perCoreStacks.Length)
					{
						num = 0;
					}
				}
				return null;
			}

			public void Trim(int currentMilliseconds, int id, Utilities.MemoryPressure pressure, int bucketSize)
			{
				TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[] perCoreStacks = this._perCoreStacks;
				for (int i = 0; i < perCoreStacks.Length; i++)
				{
					perCoreStacks[i].Trim(currentMilliseconds, id, pressure, bucketSize);
				}
			}

			private static readonly int s_lockedStackCount = Math.Min(Environment.ProcessorCount, 64);

			[Nullable(new byte[] { 1, 1, 0 })]
			private readonly TlsOverPerCoreLockedStacksArrayPool<T>.LockedStack[] _perCoreStacks;
		}

		private sealed class LockedStack
		{
			[NullableContext(1)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryPush(T[] array)
			{
				bool flag = false;
				Monitor.Enter(this);
				T[][] arrays = this._arrays;
				int count = this._count;
				if (count < arrays.Length)
				{
					if (count == 0)
					{
						this._millisecondsTimestamp = 0;
					}
					arrays[count] = array;
					this._count = count + 1;
					flag = true;
				}
				Monitor.Exit(this);
				return flag;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: Nullable(new byte[] { 2, 1 })]
			public T[] TryPop()
			{
				T[] array = null;
				Monitor.Enter(this);
				T[][] arrays = this._arrays;
				int num = this._count - 1;
				if (num < arrays.Length)
				{
					array = arrays[num];
					arrays[num] = null;
					this._count = num;
				}
				Monitor.Exit(this);
				return array;
			}

			public void Trim(int currentMilliseconds, int id, Utilities.MemoryPressure pressure, int bucketSize)
			{
				if (this._count == 0)
				{
					return;
				}
				int num = ((pressure == Utilities.MemoryPressure.High) ? 10000 : 60000);
				lock (this)
				{
					if (this._count != 0)
					{
						if (this._millisecondsTimestamp == 0)
						{
							this._millisecondsTimestamp = currentMilliseconds;
						}
						else if (currentMilliseconds - this._millisecondsTimestamp > num)
						{
							int num2 = 1;
							if (pressure != Utilities.MemoryPressure.Medium)
							{
								if (pressure == Utilities.MemoryPressure.High)
								{
									num2 = 8;
									if (bucketSize > 16384)
									{
										num2++;
									}
									if (Unsafe.SizeOf<T>() > 16)
									{
										num2++;
									}
									if (Unsafe.SizeOf<T>() > 32)
									{
										num2++;
									}
								}
							}
							else
							{
								num2 = 2;
							}
							while (this._count > 0 && num2-- > 0)
							{
								T[][] arrays = this._arrays;
								int num3 = this._count - 1;
								this._count = num3;
								object obj = arrays[num3];
								this._arrays[this._count] = null;
							}
							this._millisecondsTimestamp = ((this._count > 0) ? (this._millisecondsTimestamp + num / 4) : 0);
						}
					}
				}
			}

			[Nullable(new byte[] { 1, 2, 1 })]
			private readonly T[][] _arrays = new T[8][];

			private int _count;

			private int _millisecondsTimestamp;
		}

		private struct ThreadLocalArray
		{
			[NullableContext(1)]
			public ThreadLocalArray(T[] array)
			{
				this.Array = array;
				this.MillisecondsTimeStamp = 0;
			}

			[Nullable(new byte[] { 2, 1 })]
			public T[] Array;

			public int MillisecondsTimeStamp;
		}
	}
}
