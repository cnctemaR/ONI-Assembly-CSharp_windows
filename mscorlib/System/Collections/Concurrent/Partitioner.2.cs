using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent
{
	public static class Partitioner
	{
		public static OrderablePartitioner<TSource> Create<TSource>(IList<TSource> list, bool loadBalance)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (loadBalance)
			{
				return new Partitioner.DynamicPartitionerForIList<TSource>(list);
			}
			return new Partitioner.StaticIndexRangePartitionerForIList<TSource>(list);
		}

		public static OrderablePartitioner<TSource> Create<TSource>(TSource[] array, bool loadBalance)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (loadBalance)
			{
				return new Partitioner.DynamicPartitionerForArray<TSource>(array);
			}
			return new Partitioner.StaticIndexRangePartitionerForArray<TSource>(array);
		}

		public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source)
		{
			return Partitioner.Create<TSource>(source, EnumerablePartitionerOptions.None);
		}

		public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, EnumerablePartitionerOptions partitionerOptions)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((partitionerOptions & ~EnumerablePartitionerOptions.NoBuffering) != EnumerablePartitionerOptions.None)
			{
				throw new ArgumentOutOfRangeException("partitionerOptions");
			}
			return new Partitioner.DynamicPartitionerForIEnumerable<TSource>(source, partitionerOptions);
		}

		public static OrderablePartitioner<Tuple<long, long>> Create(long fromInclusive, long toExclusive)
		{
			int num = 3;
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			long num2 = (toExclusive - fromInclusive) / (long)(PlatformHelper.ProcessorCount * num);
			if (num2 == 0L)
			{
				num2 = 1L;
			}
			return Partitioner.Create<Tuple<long, long>>(Partitioner.CreateRanges(fromInclusive, toExclusive, num2), EnumerablePartitionerOptions.NoBuffering);
		}

		public static OrderablePartitioner<Tuple<long, long>> Create(long fromInclusive, long toExclusive, long rangeSize)
		{
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			if (rangeSize <= 0L)
			{
				throw new ArgumentOutOfRangeException("rangeSize");
			}
			return Partitioner.Create<Tuple<long, long>>(Partitioner.CreateRanges(fromInclusive, toExclusive, rangeSize), EnumerablePartitionerOptions.NoBuffering);
		}

		private static IEnumerable<Tuple<long, long>> CreateRanges(long fromInclusive, long toExclusive, long rangeSize)
		{
			bool shouldQuit = false;
			long i = fromInclusive;
			while (i < toExclusive && !shouldQuit)
			{
				long num = i;
				long num2;
				try
				{
					num2 = checked(i + rangeSize);
				}
				catch (OverflowException)
				{
					num2 = toExclusive;
					shouldQuit = true;
				}
				if (num2 > toExclusive)
				{
					num2 = toExclusive;
				}
				yield return new Tuple<long, long>(num, num2);
				i += rangeSize;
			}
			yield break;
		}

		public static OrderablePartitioner<Tuple<int, int>> Create(int fromInclusive, int toExclusive)
		{
			int num = 3;
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			int num2 = (toExclusive - fromInclusive) / (PlatformHelper.ProcessorCount * num);
			if (num2 == 0)
			{
				num2 = 1;
			}
			return Partitioner.Create<Tuple<int, int>>(Partitioner.CreateRanges(fromInclusive, toExclusive, num2), EnumerablePartitionerOptions.NoBuffering);
		}

		public static OrderablePartitioner<Tuple<int, int>> Create(int fromInclusive, int toExclusive, int rangeSize)
		{
			if (toExclusive <= fromInclusive)
			{
				throw new ArgumentOutOfRangeException("toExclusive");
			}
			if (rangeSize <= 0)
			{
				throw new ArgumentOutOfRangeException("rangeSize");
			}
			return Partitioner.Create<Tuple<int, int>>(Partitioner.CreateRanges(fromInclusive, toExclusive, rangeSize), EnumerablePartitionerOptions.NoBuffering);
		}

		private static IEnumerable<Tuple<int, int>> CreateRanges(int fromInclusive, int toExclusive, int rangeSize)
		{
			bool shouldQuit = false;
			int i = fromInclusive;
			while (i < toExclusive && !shouldQuit)
			{
				int num = i;
				int num2;
				try
				{
					num2 = checked(i + rangeSize);
				}
				catch (OverflowException)
				{
					num2 = toExclusive;
					shouldQuit = true;
				}
				if (num2 > toExclusive)
				{
					num2 = toExclusive;
				}
				yield return new Tuple<int, int>(num, num2);
				i += rangeSize;
			}
			yield break;
		}

		private static int GetDefaultChunkSize<TSource>()
		{
			int num;
			if (default(TSource) != null || Nullable.GetUnderlyingType(typeof(TSource)) != null)
			{
				num = 128;
			}
			else
			{
				num = 512 / IntPtr.Size;
			}
			return num;
		}

		private const int DEFAULT_BYTES_PER_UNIT = 128;

		private const int DEFAULT_BYTES_PER_CHUNK = 512;

		private abstract class DynamicPartitionEnumerator_Abstract<TSource, TSourceReader> : IEnumerator<KeyValuePair<long, TSource>>, IDisposable, IEnumerator
		{
			protected DynamicPartitionEnumerator_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex)
				: this(sharedReader, sharedIndex, false)
			{
			}

			protected DynamicPartitionEnumerator_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex, bool useSingleChunking)
			{
				this._sharedReader = sharedReader;
				this._sharedIndex = sharedIndex;
				this._maxChunkSize = (useSingleChunking ? 1 : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, TSourceReader>.s_defaultMaxChunkSize);
			}

			protected abstract bool GrabNextChunk(int requestedChunkSize);

			protected abstract bool HasNoElementsLeft { get; set; }

			public abstract KeyValuePair<long, TSource> Current { get; }

			public abstract void Dispose();

			public void Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (this._localOffset == null)
				{
					this._localOffset = new Partitioner.SharedInt(-1);
					this._currentChunkSize = new Partitioner.SharedInt(0);
					this._doublingCountdown = 3;
				}
				if (this._localOffset.Value < this._currentChunkSize.Value - 1)
				{
					this._localOffset.Value++;
					return true;
				}
				int num;
				if (this._currentChunkSize.Value == 0)
				{
					num = 1;
				}
				else if (this._doublingCountdown > 0)
				{
					num = this._currentChunkSize.Value;
				}
				else
				{
					num = Math.Min(this._currentChunkSize.Value * 2, this._maxChunkSize);
					this._doublingCountdown = 3;
				}
				this._doublingCountdown--;
				if (this.GrabNextChunk(num))
				{
					this._localOffset.Value = 0;
					return true;
				}
				return false;
			}

			protected readonly TSourceReader _sharedReader;

			protected static int s_defaultMaxChunkSize = Partitioner.GetDefaultChunkSize<TSource>();

			protected Partitioner.SharedInt _currentChunkSize;

			protected Partitioner.SharedInt _localOffset;

			private const int CHUNK_DOUBLING_RATE = 3;

			private int _doublingCountdown;

			protected readonly int _maxChunkSize;

			protected readonly Partitioner.SharedLong _sharedIndex;
		}

		private class DynamicPartitionerForIEnumerable<TSource> : OrderablePartitioner<TSource>
		{
			internal DynamicPartitionerForIEnumerable(IEnumerable<TSource> source, EnumerablePartitionerOptions partitionerOptions)
				: base(true, false, true)
			{
				this._source = source;
				this._useSingleChunking = (partitionerOptions & EnumerablePartitionerOptions.NoBuffering) > EnumerablePartitionerOptions.None;
			}

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				IEnumerable<KeyValuePair<long, TSource>> enumerable = new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable(this._source.GetEnumerator(), this._useSingleChunking, true);
				for (int i = 0; i < partitionCount; i++)
				{
					array[i] = enumerable.GetEnumerator();
				}
				return array;
			}

			public override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
			{
				return new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable(this._source.GetEnumerator(), this._useSingleChunking, false);
			}

			public override bool SupportsDynamicPartitions
			{
				get
				{
					return true;
				}
			}

			private IEnumerable<TSource> _source;

			private readonly bool _useSingleChunking;

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable, IDisposable
			{
				internal InternalPartitionEnumerable(IEnumerator<TSource> sharedReader, bool useSingleChunking, bool isStaticPartitioning)
				{
					this._sharedReader = sharedReader;
					this._sharedIndex = new Partitioner.SharedLong(-1L);
					this._hasNoElementsLeft = new Partitioner.SharedBool(false);
					this._sourceDepleted = new Partitioner.SharedBool(false);
					this._sharedLock = new object();
					this._useSingleChunking = useSingleChunking;
					if (!this._useSingleChunking)
					{
						int num = ((PlatformHelper.ProcessorCount > 4) ? 4 : 1);
						this._fillBuffer = new KeyValuePair<long, TSource>[num * Partitioner.GetDefaultChunkSize<TSource>()];
					}
					if (isStaticPartitioning)
					{
						this._activePartitionCount = new Partitioner.SharedInt(0);
						return;
					}
					this._activePartitionCount = null;
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					if (this._disposed)
					{
						throw new ObjectDisposedException("Can not call GetEnumerator on partitions after the source enumerable is disposed");
					}
					return new Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerator(this._sharedReader, this._sharedIndex, this._hasNoElementsLeft, this._activePartitionCount, this, this._useSingleChunking);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				private void TryCopyFromFillBuffer(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					actualNumElementsGrabbed = 0;
					KeyValuePair<long, TSource>[] fillBuffer = this._fillBuffer;
					if (fillBuffer == null)
					{
						return;
					}
					if (this._fillBufferCurrentPosition >= this._fillBufferSize)
					{
						return;
					}
					Interlocked.Increment(ref this._activeCopiers);
					int num = Interlocked.Add(ref this._fillBufferCurrentPosition, requestedChunkSize);
					int num2 = num - requestedChunkSize;
					if (num2 < this._fillBufferSize)
					{
						actualNumElementsGrabbed = ((num < this._fillBufferSize) ? num : (this._fillBufferSize - num2));
						Array.Copy(fillBuffer, num2, destArray, 0, actualNumElementsGrabbed);
					}
					Interlocked.Decrement(ref this._activeCopiers);
				}

				internal bool GrabChunk(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					actualNumElementsGrabbed = 0;
					if (this._hasNoElementsLeft.Value)
					{
						return false;
					}
					if (this._useSingleChunking)
					{
						return this.GrabChunk_Single(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
					}
					return this.GrabChunk_Buffered(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
				}

				internal bool GrabChunk_Single(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					object sharedLock = this._sharedLock;
					bool flag2;
					lock (sharedLock)
					{
						if (this._hasNoElementsLeft.Value)
						{
							flag2 = false;
						}
						else
						{
							try
							{
								if (this._sharedReader.MoveNext())
								{
									this._sharedIndex.Value = checked(this._sharedIndex.Value + 1L);
									destArray[0] = new KeyValuePair<long, TSource>(this._sharedIndex.Value, this._sharedReader.Current);
									actualNumElementsGrabbed = 1;
									flag2 = true;
								}
								else
								{
									this._sourceDepleted.Value = true;
									this._hasNoElementsLeft.Value = true;
									flag2 = false;
								}
							}
							catch
							{
								this._sourceDepleted.Value = true;
								this._hasNoElementsLeft.Value = true;
								throw;
							}
						}
					}
					return flag2;
				}

				internal bool GrabChunk_Buffered(KeyValuePair<long, TSource>[] destArray, int requestedChunkSize, ref int actualNumElementsGrabbed)
				{
					this.TryCopyFromFillBuffer(destArray, requestedChunkSize, ref actualNumElementsGrabbed);
					if (actualNumElementsGrabbed == requestedChunkSize)
					{
						return true;
					}
					if (this._sourceDepleted.Value)
					{
						this._hasNoElementsLeft.Value = true;
						this._fillBuffer = null;
						return actualNumElementsGrabbed > 0;
					}
					object sharedLock = this._sharedLock;
					lock (sharedLock)
					{
						if (this._sourceDepleted.Value)
						{
							return actualNumElementsGrabbed > 0;
						}
						try
						{
							if (this._activeCopiers > 0)
							{
								SpinWait spinWait = default(SpinWait);
								while (this._activeCopiers > 0)
								{
									spinWait.SpinOnce();
								}
							}
							while (actualNumElementsGrabbed < requestedChunkSize)
							{
								if (!this._sharedReader.MoveNext())
								{
									this._sourceDepleted.Value = true;
									break;
								}
								this._sharedIndex.Value = checked(this._sharedIndex.Value + 1L);
								destArray[actualNumElementsGrabbed] = new KeyValuePair<long, TSource>(this._sharedIndex.Value, this._sharedReader.Current);
								actualNumElementsGrabbed++;
							}
							KeyValuePair<long, TSource>[] fillBuffer = this._fillBuffer;
							if (!this._sourceDepleted.Value && fillBuffer != null && this._fillBufferCurrentPosition >= fillBuffer.Length)
							{
								for (int i = 0; i < fillBuffer.Length; i++)
								{
									if (!this._sharedReader.MoveNext())
									{
										this._sourceDepleted.Value = true;
										this._fillBufferSize = i;
										break;
									}
									this._sharedIndex.Value = checked(this._sharedIndex.Value + 1L);
									fillBuffer[i] = new KeyValuePair<long, TSource>(this._sharedIndex.Value, this._sharedReader.Current);
								}
								this._fillBufferCurrentPosition = 0;
							}
						}
						catch
						{
							this._sourceDepleted.Value = true;
							this._hasNoElementsLeft.Value = true;
							throw;
						}
					}
					return actualNumElementsGrabbed > 0;
				}

				public void Dispose()
				{
					if (!this._disposed)
					{
						this._disposed = true;
						this._sharedReader.Dispose();
					}
				}

				private readonly IEnumerator<TSource> _sharedReader;

				private Partitioner.SharedLong _sharedIndex;

				private volatile KeyValuePair<long, TSource>[] _fillBuffer;

				private volatile int _fillBufferSize;

				private volatile int _fillBufferCurrentPosition;

				private volatile int _activeCopiers;

				private Partitioner.SharedBool _hasNoElementsLeft;

				private Partitioner.SharedBool _sourceDepleted;

				private object _sharedLock;

				private bool _disposed;

				private Partitioner.SharedInt _activePartitionCount;

				private readonly bool _useSingleChunking;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, IEnumerator<TSource>>
			{
				internal InternalPartitionEnumerator(IEnumerator<TSource> sharedReader, Partitioner.SharedLong sharedIndex, Partitioner.SharedBool hasNoElementsLeft, Partitioner.SharedInt activePartitionCount, Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable enumerable, bool useSingleChunking)
					: base(sharedReader, sharedIndex, useSingleChunking)
				{
					this._hasNoElementsLeft = hasNoElementsLeft;
					this._enumerable = enumerable;
					this._activePartitionCount = activePartitionCount;
					if (this._activePartitionCount != null)
					{
						Interlocked.Increment(ref this._activePartitionCount.Value);
					}
				}

				protected override bool GrabNextChunk(int requestedChunkSize)
				{
					if (this.HasNoElementsLeft)
					{
						return false;
					}
					if (this._localList == null)
					{
						this._localList = new KeyValuePair<long, TSource>[this._maxChunkSize];
					}
					return this._enumerable.GrabChunk(this._localList, requestedChunkSize, ref this._currentChunkSize.Value);
				}

				protected override bool HasNoElementsLeft
				{
					get
					{
						return this._hasNoElementsLeft.Value;
					}
					set
					{
						this._hasNoElementsLeft.Value = true;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this._currentChunkSize == null)
						{
							throw new InvalidOperationException("MoveNext must be called at least once before calling Current.");
						}
						return this._localList[this._localOffset.Value];
					}
				}

				public override void Dispose()
				{
					if (this._activePartitionCount != null && Interlocked.Decrement(ref this._activePartitionCount.Value) == 0)
					{
						this._enumerable.Dispose();
					}
				}

				private KeyValuePair<long, TSource>[] _localList;

				private readonly Partitioner.SharedBool _hasNoElementsLeft;

				private readonly Partitioner.SharedInt _activePartitionCount;

				private Partitioner.DynamicPartitionerForIEnumerable<TSource>.InternalPartitionEnumerable _enumerable;
			}
		}

		private abstract class DynamicPartitionerForIndexRange_Abstract<TSource, TCollection> : OrderablePartitioner<TSource>
		{
			protected DynamicPartitionerForIndexRange_Abstract(TCollection data)
				: base(true, false, true)
			{
				this._data = data;
			}

			protected abstract IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(TCollection data);

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				IEnumerable<KeyValuePair<long, TSource>> orderableDynamicPartitions_Factory = this.GetOrderableDynamicPartitions_Factory(this._data);
				for (int i = 0; i < partitionCount; i++)
				{
					array[i] = orderableDynamicPartitions_Factory.GetEnumerator();
				}
				return array;
			}

			public override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
			{
				return this.GetOrderableDynamicPartitions_Factory(this._data);
			}

			public override bool SupportsDynamicPartitions
			{
				get
				{
					return true;
				}
			}

			private TCollection _data;
		}

		private abstract class DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, TSourceReader> : Partitioner.DynamicPartitionEnumerator_Abstract<TSource, TSourceReader>
		{
			protected DynamicPartitionEnumeratorForIndexRange_Abstract(TSourceReader sharedReader, Partitioner.SharedLong sharedIndex)
				: base(sharedReader, sharedIndex)
			{
			}

			protected abstract int SourceCount { get; }

			protected override bool GrabNextChunk(int requestedChunkSize)
			{
				while (!this.HasNoElementsLeft)
				{
					long num = Volatile.Read(ref this._sharedIndex.Value);
					if (this.HasNoElementsLeft)
					{
						return false;
					}
					long num2 = Math.Min((long)(this.SourceCount - 1), num + (long)requestedChunkSize);
					if (Interlocked.CompareExchange(ref this._sharedIndex.Value, num2, num) == num)
					{
						this._currentChunkSize.Value = (int)(num2 - num);
						this._localOffset.Value = -1;
						this._startIndex = (int)(num + 1L);
						return true;
					}
				}
				return false;
			}

			protected override bool HasNoElementsLeft
			{
				get
				{
					return Volatile.Read(ref this._sharedIndex.Value) >= (long)(this.SourceCount - 1);
				}
				set
				{
				}
			}

			public override void Dispose()
			{
			}

			protected int _startIndex;
		}

		private class DynamicPartitionerForIList<TSource> : Partitioner.DynamicPartitionerForIndexRange_Abstract<TSource, IList<TSource>>
		{
			internal DynamicPartitionerForIList(IList<TSource> source)
				: base(source)
			{
			}

			protected override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(IList<TSource> _data)
			{
				return new Partitioner.DynamicPartitionerForIList<TSource>.InternalPartitionEnumerable(_data);
			}

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable
			{
				internal InternalPartitionEnumerable(IList<TSource> sharedReader)
				{
					this._sharedReader = sharedReader;
					this._sharedIndex = new Partitioner.SharedLong(-1L);
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					return new Partitioner.DynamicPartitionerForIList<TSource>.InternalPartitionEnumerator(this._sharedReader, this._sharedIndex);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				private readonly IList<TSource> _sharedReader;

				private Partitioner.SharedLong _sharedIndex;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, IList<TSource>>
			{
				internal InternalPartitionEnumerator(IList<TSource> sharedReader, Partitioner.SharedLong sharedIndex)
					: base(sharedReader, sharedIndex)
				{
				}

				protected override int SourceCount
				{
					get
					{
						return this._sharedReader.Count;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this._currentChunkSize == null)
						{
							throw new InvalidOperationException("MoveNext must be called at least once before calling Current.");
						}
						return new KeyValuePair<long, TSource>((long)(this._startIndex + this._localOffset.Value), this._sharedReader[this._startIndex + this._localOffset.Value]);
					}
				}
			}
		}

		private class DynamicPartitionerForArray<TSource> : Partitioner.DynamicPartitionerForIndexRange_Abstract<TSource, TSource[]>
		{
			internal DynamicPartitionerForArray(TSource[] source)
				: base(source)
			{
			}

			protected override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions_Factory(TSource[] _data)
			{
				return new Partitioner.DynamicPartitionerForArray<TSource>.InternalPartitionEnumerable(_data);
			}

			private class InternalPartitionEnumerable : IEnumerable<KeyValuePair<long, TSource>>, IEnumerable
			{
				internal InternalPartitionEnumerable(TSource[] sharedReader)
				{
					this._sharedReader = sharedReader;
					this._sharedIndex = new Partitioner.SharedLong(-1L);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
				{
					return new Partitioner.DynamicPartitionerForArray<TSource>.InternalPartitionEnumerator(this._sharedReader, this._sharedIndex);
				}

				private readonly TSource[] _sharedReader;

				private Partitioner.SharedLong _sharedIndex;
			}

			private class InternalPartitionEnumerator : Partitioner.DynamicPartitionEnumeratorForIndexRange_Abstract<TSource, TSource[]>
			{
				internal InternalPartitionEnumerator(TSource[] sharedReader, Partitioner.SharedLong sharedIndex)
					: base(sharedReader, sharedIndex)
				{
				}

				protected override int SourceCount
				{
					get
					{
						return this._sharedReader.Length;
					}
				}

				public override KeyValuePair<long, TSource> Current
				{
					get
					{
						if (this._currentChunkSize == null)
						{
							throw new InvalidOperationException("MoveNext must be called at least once before calling Current.");
						}
						return new KeyValuePair<long, TSource>((long)(this._startIndex + this._localOffset.Value), this._sharedReader[this._startIndex + this._localOffset.Value]);
					}
				}
			}
		}

		private abstract class StaticIndexRangePartitioner<TSource, TCollection> : OrderablePartitioner<TSource>
		{
			protected StaticIndexRangePartitioner()
				: base(true, true, true)
			{
			}

			protected abstract int SourceCount { get; }

			protected abstract IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex);

			public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
			{
				if (partitionCount <= 0)
				{
					throw new ArgumentOutOfRangeException("partitionCount");
				}
				int num = this.SourceCount / partitionCount;
				int num2 = this.SourceCount % partitionCount;
				IEnumerator<KeyValuePair<long, TSource>>[] array = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];
				int num3 = -1;
				for (int i = 0; i < partitionCount; i++)
				{
					int num4 = num3 + 1;
					if (i < num2)
					{
						num3 = num4 + num;
					}
					else
					{
						num3 = num4 + num - 1;
					}
					array[i] = this.CreatePartition(num4, num3);
				}
				return array;
			}
		}

		private abstract class StaticIndexRangePartition<TSource> : IEnumerator<KeyValuePair<long, TSource>>, IDisposable, IEnumerator
		{
			protected StaticIndexRangePartition(int startIndex, int endIndex)
			{
				this._startIndex = startIndex;
				this._endIndex = endIndex;
				this._offset = startIndex - 1;
			}

			public abstract KeyValuePair<long, TSource> Current { get; }

			public void Dispose()
			{
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public bool MoveNext()
			{
				if (this._offset < this._endIndex)
				{
					this._offset++;
					return true;
				}
				this._offset = this._endIndex + 1;
				return false;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			protected readonly int _startIndex;

			protected readonly int _endIndex;

			protected volatile int _offset;
		}

		private class StaticIndexRangePartitionerForIList<TSource> : Partitioner.StaticIndexRangePartitioner<TSource, IList<TSource>>
		{
			internal StaticIndexRangePartitionerForIList(IList<TSource> list)
			{
				this._list = list;
			}

			protected override int SourceCount
			{
				get
				{
					return this._list.Count;
				}
			}

			protected override IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex)
			{
				return new Partitioner.StaticIndexRangePartitionForIList<TSource>(this._list, startIndex, endIndex);
			}

			private IList<TSource> _list;
		}

		private class StaticIndexRangePartitionForIList<TSource> : Partitioner.StaticIndexRangePartition<TSource>
		{
			internal StaticIndexRangePartitionForIList(IList<TSource> list, int startIndex, int endIndex)
				: base(startIndex, endIndex)
			{
				this._list = list;
			}

			public override KeyValuePair<long, TSource> Current
			{
				get
				{
					if (this._offset < this._startIndex)
					{
						throw new InvalidOperationException("MoveNext must be called at least once before calling Current.");
					}
					return new KeyValuePair<long, TSource>((long)this._offset, this._list[this._offset]);
				}
			}

			private volatile IList<TSource> _list;
		}

		private class StaticIndexRangePartitionerForArray<TSource> : Partitioner.StaticIndexRangePartitioner<TSource, TSource[]>
		{
			internal StaticIndexRangePartitionerForArray(TSource[] array)
			{
				this._array = array;
			}

			protected override int SourceCount
			{
				get
				{
					return this._array.Length;
				}
			}

			protected override IEnumerator<KeyValuePair<long, TSource>> CreatePartition(int startIndex, int endIndex)
			{
				return new Partitioner.StaticIndexRangePartitionForArray<TSource>(this._array, startIndex, endIndex);
			}

			private TSource[] _array;
		}

		private class StaticIndexRangePartitionForArray<TSource> : Partitioner.StaticIndexRangePartition<TSource>
		{
			internal StaticIndexRangePartitionForArray(TSource[] array, int startIndex, int endIndex)
				: base(startIndex, endIndex)
			{
				this._array = array;
			}

			public override KeyValuePair<long, TSource> Current
			{
				get
				{
					if (this._offset < this._startIndex)
					{
						throw new InvalidOperationException("MoveNext must be called at least once before calling Current.");
					}
					return new KeyValuePair<long, TSource>((long)this._offset, this._array[this._offset]);
				}
			}

			private volatile TSource[] _array;
		}

		private class SharedInt
		{
			internal SharedInt(int value)
			{
				this.Value = value;
			}

			internal volatile int Value;
		}

		private class SharedBool
		{
			internal SharedBool(bool value)
			{
				this.Value = value;
			}

			internal volatile bool Value;
		}

		private class SharedLong
		{
			internal SharedLong(long value)
			{
				this.Value = value;
			}

			internal long Value;
		}
	}
}
