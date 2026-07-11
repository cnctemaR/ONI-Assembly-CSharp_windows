using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class OrderPreservingPipeliningMergeHelper<TOutput, TKey> : IMergeHelper<TOutput>
	{
		internal OrderPreservingPipeliningMergeHelper(PartitionedStream<TOutput, TKey> partitions, TaskScheduler taskScheduler, CancellationState cancellationState, bool autoBuffered, int queryId, IComparer<TKey> keyComparer)
		{
			this._taskGroupState = new QueryTaskGroupState(cancellationState, queryId);
			this._partitions = partitions;
			this._taskScheduler = taskScheduler;
			this._autoBuffered = autoBuffered;
			int partitionCount = this._partitions.PartitionCount;
			this._buffers = new Queue<Pair<TKey, TOutput>>[partitionCount];
			this._producerDone = new bool[partitionCount];
			this._consumerWaiting = new bool[partitionCount];
			this._producerWaiting = new bool[partitionCount];
			this._bufferLocks = new object[partitionCount];
			if (keyComparer == Util.GetDefaultComparer<int>())
			{
				this._producerComparer = (IComparer<Producer<TKey>>)new ProducerComparerInt();
				return;
			}
			this._producerComparer = new OrderPreservingPipeliningMergeHelper<TOutput, TKey>.ProducerComparer(keyComparer);
		}

		void IMergeHelper<TOutput>.Execute()
		{
			OrderPreservingPipeliningSpoolingTask<TOutput, TKey>.Spool(this._taskGroupState, this._partitions, this._consumerWaiting, this._producerWaiting, this._producerDone, this._buffers, this._bufferLocks, this._taskScheduler, this._autoBuffered);
		}

		IEnumerator<TOutput> IMergeHelper<TOutput>.GetEnumerator()
		{
			return new OrderPreservingPipeliningMergeHelper<TOutput, TKey>.OrderedPipeliningMergeEnumerator(this, this._producerComparer);
		}

		[ExcludeFromCodeCoverage]
		public TOutput[] GetResultsAsArray()
		{
			throw new InvalidOperationException();
		}

		private readonly QueryTaskGroupState _taskGroupState;

		private readonly PartitionedStream<TOutput, TKey> _partitions;

		private readonly TaskScheduler _taskScheduler;

		private readonly bool _autoBuffered;

		private readonly Queue<Pair<TKey, TOutput>>[] _buffers;

		private readonly bool[] _producerDone;

		private readonly bool[] _producerWaiting;

		private readonly bool[] _consumerWaiting;

		private readonly object[] _bufferLocks;

		private IComparer<Producer<TKey>> _producerComparer;

		internal const int INITIAL_BUFFER_SIZE = 128;

		internal const int STEAL_BUFFER_SIZE = 1024;

		internal const int MAX_BUFFER_SIZE = 8192;

		private class ProducerComparer : IComparer<Producer<TKey>>
		{
			internal ProducerComparer(IComparer<TKey> keyComparer)
			{
				this._keyComparer = keyComparer;
			}

			public int Compare(Producer<TKey> x, Producer<TKey> y)
			{
				return this._keyComparer.Compare(y.MaxKey, x.MaxKey);
			}

			private IComparer<TKey> _keyComparer;
		}

		private class OrderedPipeliningMergeEnumerator : MergeEnumerator<TOutput>
		{
			internal OrderedPipeliningMergeEnumerator(OrderPreservingPipeliningMergeHelper<TOutput, TKey> mergeHelper, IComparer<Producer<TKey>> producerComparer)
				: base(mergeHelper._taskGroupState)
			{
				int partitionCount = mergeHelper._partitions.PartitionCount;
				this._mergeHelper = mergeHelper;
				this._producerHeap = new FixedMaxHeap<Producer<TKey>>(partitionCount, producerComparer);
				this._privateBuffer = new Queue<Pair<TKey, TOutput>>[partitionCount];
				this._producerNextElement = new TOutput[partitionCount];
			}

			public override TOutput Current
			{
				get
				{
					int producerIndex = this._producerHeap.MaxValue.ProducerIndex;
					return this._producerNextElement[producerIndex];
				}
			}

			public override bool MoveNext()
			{
				if (!this._initialized)
				{
					this._initialized = true;
					for (int i = 0; i < this._mergeHelper._partitions.PartitionCount; i++)
					{
						Pair<TKey, TOutput> pair = default(Pair<TKey, TOutput>);
						if (this.TryWaitForElement(i, ref pair))
						{
							this._producerHeap.Insert(new Producer<TKey>(pair.First, i));
							this._producerNextElement[i] = pair.Second;
						}
						else
						{
							this.ThrowIfInTearDown();
						}
					}
				}
				else
				{
					if (this._producerHeap.Count == 0)
					{
						return false;
					}
					int producerIndex = this._producerHeap.MaxValue.ProducerIndex;
					Pair<TKey, TOutput> pair2 = default(Pair<TKey, TOutput>);
					if (this.TryGetPrivateElement(producerIndex, ref pair2) || this.TryWaitForElement(producerIndex, ref pair2))
					{
						this._producerHeap.ReplaceMax(new Producer<TKey>(pair2.First, producerIndex));
						this._producerNextElement[producerIndex] = pair2.Second;
					}
					else
					{
						this.ThrowIfInTearDown();
						this._producerHeap.RemoveMax();
					}
				}
				return this._producerHeap.Count > 0;
			}

			private void ThrowIfInTearDown()
			{
				if (this._mergeHelper._taskGroupState.CancellationState.MergedCancellationToken.IsCancellationRequested)
				{
					try
					{
						object[] bufferLocks = this._mergeHelper._bufferLocks;
						for (int i = 0; i < bufferLocks.Length; i++)
						{
							object obj = bufferLocks[i];
							lock (obj)
							{
								Monitor.Pulse(bufferLocks[i]);
							}
						}
						this._taskGroupState.QueryEnd(false);
					}
					finally
					{
						this._producerHeap.Clear();
					}
				}
			}

			private bool TryWaitForElement(int producer, ref Pair<TKey, TOutput> element)
			{
				Queue<Pair<TKey, TOutput>> queue = this._mergeHelper._buffers[producer];
				object obj = this._mergeHelper._bufferLocks[producer];
				object obj2 = obj;
				lock (obj2)
				{
					if (queue.Count == 0)
					{
						if (this._mergeHelper._producerDone[producer])
						{
							element = default(Pair<TKey, TOutput>);
							return false;
						}
						this._mergeHelper._consumerWaiting[producer] = true;
						Monitor.Wait(obj);
						if (queue.Count == 0)
						{
							element = default(Pair<TKey, TOutput>);
							return false;
						}
					}
					if (this._mergeHelper._producerWaiting[producer])
					{
						Monitor.Pulse(obj);
						this._mergeHelper._producerWaiting[producer] = false;
					}
					if (queue.Count < 1024)
					{
						element = queue.Dequeue();
						return true;
					}
					this._privateBuffer[producer] = this._mergeHelper._buffers[producer];
					this._mergeHelper._buffers[producer] = new Queue<Pair<TKey, TOutput>>(128);
				}
				this.TryGetPrivateElement(producer, ref element);
				return true;
			}

			private bool TryGetPrivateElement(int producer, ref Pair<TKey, TOutput> element)
			{
				Queue<Pair<TKey, TOutput>> queue = this._privateBuffer[producer];
				if (queue != null)
				{
					if (queue.Count > 0)
					{
						element = queue.Dequeue();
						return true;
					}
					this._privateBuffer[producer] = null;
				}
				return false;
			}

			public override void Dispose()
			{
				int num = this._mergeHelper._buffers.Length;
				for (int i = 0; i < num; i++)
				{
					object obj = this._mergeHelper._bufferLocks[i];
					object obj2 = obj;
					lock (obj2)
					{
						if (this._mergeHelper._producerWaiting[i])
						{
							Monitor.Pulse(obj);
						}
					}
				}
				base.Dispose();
			}

			private OrderPreservingPipeliningMergeHelper<TOutput, TKey> _mergeHelper;

			private readonly FixedMaxHeap<Producer<TKey>> _producerHeap;

			private readonly TOutput[] _producerNextElement;

			private readonly Queue<Pair<TKey, TOutput>>[] _privateBuffer;

			private bool _initialized;
		}
	}
}
