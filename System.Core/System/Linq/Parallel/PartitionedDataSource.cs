using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class PartitionedDataSource<T> : PartitionedStream<T, int>
	{
		internal PartitionedDataSource(IEnumerable<T> source, int partitionCount, bool useStriping)
			: base(partitionCount, Util.GetDefaultComparer<int>(), (source is IList<T>) ? OrdinalIndexState.Indexable : OrdinalIndexState.Correct)
		{
			this.InitializePartitions(source, partitionCount, useStriping);
		}

		private void InitializePartitions(IEnumerable<T> source, int partitionCount, bool useStriping)
		{
			ParallelEnumerableWrapper<T> parallelEnumerableWrapper = source as ParallelEnumerableWrapper<T>;
			if (parallelEnumerableWrapper != null)
			{
				source = parallelEnumerableWrapper.WrappedEnumerable;
			}
			IList<T> list = source as IList<T>;
			if (list != null)
			{
				QueryOperatorEnumerator<T, int>[] array = new QueryOperatorEnumerator<T, int>[partitionCount];
				T[] array2 = source as T[];
				int num = -1;
				if (useStriping)
				{
					num = Scheduling.GetDefaultChunkSize<T>();
					if (num < 1)
					{
						num = 1;
					}
				}
				for (int i = 0; i < partitionCount; i++)
				{
					if (array2 != null)
					{
						if (useStriping)
						{
							array[i] = new PartitionedDataSource<T>.ArrayIndexRangeEnumerator(array2, partitionCount, i, num);
						}
						else
						{
							array[i] = new PartitionedDataSource<T>.ArrayContiguousIndexRangeEnumerator(array2, partitionCount, i);
						}
					}
					else if (useStriping)
					{
						array[i] = new PartitionedDataSource<T>.ListIndexRangeEnumerator(list, partitionCount, i, num);
					}
					else
					{
						array[i] = new PartitionedDataSource<T>.ListContiguousIndexRangeEnumerator(list, partitionCount, i);
					}
				}
				this._partitions = array;
				return;
			}
			this._partitions = PartitionedDataSource<T>.MakePartitions(source.GetEnumerator(), partitionCount);
		}

		private static QueryOperatorEnumerator<T, int>[] MakePartitions(IEnumerator<T> source, int partitionCount)
		{
			QueryOperatorEnumerator<T, int>[] array = new QueryOperatorEnumerator<T, int>[partitionCount];
			object obj = new object();
			Shared<int> shared = new Shared<int>(0);
			Shared<int> shared2 = new Shared<int>(partitionCount);
			Shared<bool> shared3 = new Shared<bool>(false);
			for (int i = 0; i < partitionCount; i++)
			{
				array[i] = new PartitionedDataSource<T>.ContiguousChunkLazyEnumerator(source, shared3, obj, shared, shared2);
			}
			return array;
		}

		internal sealed class ArrayIndexRangeEnumerator : QueryOperatorEnumerator<T, int>
		{
			internal ArrayIndexRangeEnumerator(T[] data, int partitionCount, int partitionIndex, int maxChunkSize)
			{
				this._data = data;
				this._elementCount = data.Length;
				this._partitionCount = partitionCount;
				this._partitionIndex = partitionIndex;
				this._maxChunkSize = maxChunkSize;
				int num = maxChunkSize * partitionCount;
				this._sectionCount = this._elementCount / num + ((this._elementCount % num == 0) ? 0 : 1);
			}

			internal override bool MoveNext(ref T currentElement, ref int currentKey)
			{
				PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables mutables = this._mutables;
				if (mutables == null)
				{
					mutables = (this._mutables = new PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables());
				}
				PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables mutables2 = mutables;
				int num = mutables2._currentPositionInChunk + 1;
				mutables2._currentPositionInChunk = num;
				if (num < mutables._currentChunkSize || this.MoveNextSlowPath())
				{
					currentKey = mutables._currentChunkOffset + mutables._currentPositionInChunk;
					currentElement = this._data[currentKey];
					return true;
				}
				return false;
			}

			private bool MoveNextSlowPath()
			{
				PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables mutables = this._mutables;
				PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables mutables2 = mutables;
				int num = mutables2._currentSection + 1;
				mutables2._currentSection = num;
				int num2 = num;
				int num3 = this._sectionCount - num2;
				if (num3 <= 0)
				{
					return false;
				}
				int num4 = num2 * this._partitionCount * this._maxChunkSize;
				mutables._currentPositionInChunk = 0;
				if (num3 > 1)
				{
					mutables._currentChunkSize = this._maxChunkSize;
					mutables._currentChunkOffset = num4 + this._partitionIndex * this._maxChunkSize;
				}
				else
				{
					int num5 = this._elementCount - num4;
					int num6 = num5 / this._partitionCount;
					int num7 = num5 % this._partitionCount;
					mutables._currentChunkSize = num6;
					if (this._partitionIndex < num7)
					{
						mutables._currentChunkSize++;
					}
					if (mutables._currentChunkSize == 0)
					{
						return false;
					}
					mutables._currentChunkOffset = num4 + this._partitionIndex * num6 + ((this._partitionIndex < num7) ? this._partitionIndex : num7);
				}
				return true;
			}

			private readonly T[] _data;

			private readonly int _elementCount;

			private readonly int _partitionCount;

			private readonly int _partitionIndex;

			private readonly int _maxChunkSize;

			private readonly int _sectionCount;

			private PartitionedDataSource<T>.ArrayIndexRangeEnumerator.Mutables _mutables;

			private class Mutables
			{
				internal Mutables()
				{
					this._currentSection = -1;
				}

				internal int _currentSection;

				internal int _currentChunkSize;

				internal int _currentPositionInChunk;

				internal int _currentChunkOffset;
			}
		}

		internal sealed class ArrayContiguousIndexRangeEnumerator : QueryOperatorEnumerator<T, int>
		{
			internal ArrayContiguousIndexRangeEnumerator(T[] data, int partitionCount, int partitionIndex)
			{
				this._data = data;
				int num = data.Length / partitionCount;
				int num2 = data.Length % partitionCount;
				int num3 = partitionIndex * num + ((partitionIndex < num2) ? partitionIndex : num2);
				this._startIndex = num3 - 1;
				this._maximumIndex = num3 + num + ((partitionIndex < num2) ? 1 : 0);
			}

			internal override bool MoveNext(ref T currentElement, ref int currentKey)
			{
				if (this._currentIndex == null)
				{
					this._currentIndex = new Shared<int>(this._startIndex);
				}
				Shared<int> currentIndex = this._currentIndex;
				int num = currentIndex.Value + 1;
				currentIndex.Value = num;
				int num2 = num;
				if (num2 < this._maximumIndex)
				{
					currentKey = num2;
					currentElement = this._data[num2];
					return true;
				}
				return false;
			}

			private readonly T[] _data;

			private readonly int _startIndex;

			private readonly int _maximumIndex;

			private Shared<int> _currentIndex;
		}

		internal sealed class ListIndexRangeEnumerator : QueryOperatorEnumerator<T, int>
		{
			internal ListIndexRangeEnumerator(IList<T> data, int partitionCount, int partitionIndex, int maxChunkSize)
			{
				this._data = data;
				this._elementCount = data.Count;
				this._partitionCount = partitionCount;
				this._partitionIndex = partitionIndex;
				this._maxChunkSize = maxChunkSize;
				int num = maxChunkSize * partitionCount;
				this._sectionCount = this._elementCount / num + ((this._elementCount % num == 0) ? 0 : 1);
			}

			internal override bool MoveNext(ref T currentElement, ref int currentKey)
			{
				PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables mutables = this._mutables;
				if (mutables == null)
				{
					mutables = (this._mutables = new PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables());
				}
				PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables mutables2 = mutables;
				int num = mutables2._currentPositionInChunk + 1;
				mutables2._currentPositionInChunk = num;
				if (num < mutables._currentChunkSize || this.MoveNextSlowPath())
				{
					currentKey = mutables._currentChunkOffset + mutables._currentPositionInChunk;
					currentElement = this._data[currentKey];
					return true;
				}
				return false;
			}

			private bool MoveNextSlowPath()
			{
				PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables mutables = this._mutables;
				PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables mutables2 = mutables;
				int num = mutables2._currentSection + 1;
				mutables2._currentSection = num;
				int num2 = num;
				int num3 = this._sectionCount - num2;
				if (num3 <= 0)
				{
					return false;
				}
				int num4 = num2 * this._partitionCount * this._maxChunkSize;
				mutables._currentPositionInChunk = 0;
				if (num3 > 1)
				{
					mutables._currentChunkSize = this._maxChunkSize;
					mutables._currentChunkOffset = num4 + this._partitionIndex * this._maxChunkSize;
				}
				else
				{
					int num5 = this._elementCount - num4;
					int num6 = num5 / this._partitionCount;
					int num7 = num5 % this._partitionCount;
					mutables._currentChunkSize = num6;
					if (this._partitionIndex < num7)
					{
						mutables._currentChunkSize++;
					}
					if (mutables._currentChunkSize == 0)
					{
						return false;
					}
					mutables._currentChunkOffset = num4 + this._partitionIndex * num6 + ((this._partitionIndex < num7) ? this._partitionIndex : num7);
				}
				return true;
			}

			private readonly IList<T> _data;

			private readonly int _elementCount;

			private readonly int _partitionCount;

			private readonly int _partitionIndex;

			private readonly int _maxChunkSize;

			private readonly int _sectionCount;

			private PartitionedDataSource<T>.ListIndexRangeEnumerator.Mutables _mutables;

			private class Mutables
			{
				internal Mutables()
				{
					this._currentSection = -1;
				}

				internal int _currentSection;

				internal int _currentChunkSize;

				internal int _currentPositionInChunk;

				internal int _currentChunkOffset;
			}
		}

		internal sealed class ListContiguousIndexRangeEnumerator : QueryOperatorEnumerator<T, int>
		{
			internal ListContiguousIndexRangeEnumerator(IList<T> data, int partitionCount, int partitionIndex)
			{
				this._data = data;
				int num = data.Count / partitionCount;
				int num2 = data.Count % partitionCount;
				int num3 = partitionIndex * num + ((partitionIndex < num2) ? partitionIndex : num2);
				this._startIndex = num3 - 1;
				this._maximumIndex = num3 + num + ((partitionIndex < num2) ? 1 : 0);
			}

			internal override bool MoveNext(ref T currentElement, ref int currentKey)
			{
				if (this._currentIndex == null)
				{
					this._currentIndex = new Shared<int>(this._startIndex);
				}
				Shared<int> currentIndex = this._currentIndex;
				int num = currentIndex.Value + 1;
				currentIndex.Value = num;
				int num2 = num;
				if (num2 < this._maximumIndex)
				{
					currentKey = num2;
					currentElement = this._data[num2];
					return true;
				}
				return false;
			}

			private readonly IList<T> _data;

			private readonly int _startIndex;

			private readonly int _maximumIndex;

			private Shared<int> _currentIndex;
		}

		private class ContiguousChunkLazyEnumerator : QueryOperatorEnumerator<T, int>
		{
			internal ContiguousChunkLazyEnumerator(IEnumerator<T> source, Shared<bool> exceptionTracker, object sourceSyncLock, Shared<int> currentIndex, Shared<int> degreeOfParallelism)
			{
				this._source = source;
				this._sourceSyncLock = sourceSyncLock;
				this._currentIndex = currentIndex;
				this._activeEnumeratorsCount = degreeOfParallelism;
				this._exceptionTracker = exceptionTracker;
			}

			internal override bool MoveNext(ref T currentElement, ref int currentKey)
			{
				PartitionedDataSource<T>.ContiguousChunkLazyEnumerator.Mutables mutables = this._mutables;
				if (mutables == null)
				{
					mutables = (this._mutables = new PartitionedDataSource<T>.ContiguousChunkLazyEnumerator.Mutables());
				}
				T[] chunkBuffer;
				int num2;
				for (;;)
				{
					chunkBuffer = mutables._chunkBuffer;
					PartitionedDataSource<T>.ContiguousChunkLazyEnumerator.Mutables mutables2 = mutables;
					int num = mutables2._currentChunkIndex + 1;
					mutables2._currentChunkIndex = num;
					num2 = num;
					if (num2 < mutables._currentChunkSize)
					{
						break;
					}
					object sourceSyncLock = this._sourceSyncLock;
					lock (sourceSyncLock)
					{
						int num3 = 0;
						if (this._exceptionTracker.Value)
						{
							return false;
						}
						try
						{
							while (num3 < mutables._nextChunkMaxSize && this._source.MoveNext())
							{
								chunkBuffer[num3] = this._source.Current;
								num3++;
							}
						}
						catch
						{
							this._exceptionTracker.Value = true;
							throw;
						}
						mutables._currentChunkSize = num3;
						if (num3 == 0)
						{
							return false;
						}
						mutables._chunkBaseIndex = this._currentIndex.Value;
						checked
						{
							this._currentIndex.Value += num3;
						}
					}
					if (mutables._nextChunkMaxSize < chunkBuffer.Length)
					{
						PartitionedDataSource<T>.ContiguousChunkLazyEnumerator.Mutables mutables3 = mutables;
						num = mutables3._chunkCounter;
						mutables3._chunkCounter = num + 1;
						if ((num & 7) == 7)
						{
							mutables._nextChunkMaxSize *= 2;
							if (mutables._nextChunkMaxSize > chunkBuffer.Length)
							{
								mutables._nextChunkMaxSize = chunkBuffer.Length;
							}
						}
					}
					mutables._currentChunkIndex = -1;
				}
				currentElement = chunkBuffer[num2];
				currentKey = mutables._chunkBaseIndex + num2;
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				if (Interlocked.Decrement(ref this._activeEnumeratorsCount.Value) == 0)
				{
					this._source.Dispose();
				}
			}

			private const int chunksPerChunkSize = 7;

			private readonly IEnumerator<T> _source;

			private readonly object _sourceSyncLock;

			private readonly Shared<int> _currentIndex;

			private readonly Shared<int> _activeEnumeratorsCount;

			private readonly Shared<bool> _exceptionTracker;

			private PartitionedDataSource<T>.ContiguousChunkLazyEnumerator.Mutables _mutables;

			private class Mutables
			{
				internal Mutables()
				{
					this._nextChunkMaxSize = 1;
					this._chunkBuffer = new T[Scheduling.GetDefaultChunkSize<T>()];
					this._currentChunkSize = 0;
					this._currentChunkIndex = -1;
					this._chunkBaseIndex = 0;
					this._chunkCounter = 0;
				}

				internal readonly T[] _chunkBuffer;

				internal int _nextChunkMaxSize;

				internal int _currentChunkSize;

				internal int _currentChunkIndex;

				internal int _chunkBaseIndex;

				internal int _chunkCounter;
			}
		}
	}
}
