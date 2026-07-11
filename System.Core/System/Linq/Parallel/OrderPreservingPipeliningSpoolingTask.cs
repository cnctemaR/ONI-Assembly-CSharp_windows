using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class OrderPreservingPipeliningSpoolingTask<TOutput, TKey> : SpoolingTaskBase
	{
		internal OrderPreservingPipeliningSpoolingTask(QueryOperatorEnumerator<TOutput, TKey> partition, QueryTaskGroupState taskGroupState, bool[] consumerWaiting, bool[] producerWaiting, bool[] producerDone, int partitionIndex, Queue<Pair<TKey, TOutput>>[] buffers, object bufferLock, bool autoBuffered)
			: base(partitionIndex, taskGroupState)
		{
			this._partition = partition;
			this._taskGroupState = taskGroupState;
			this._producerDone = producerDone;
			this._consumerWaiting = consumerWaiting;
			this._producerWaiting = producerWaiting;
			this._partitionIndex = partitionIndex;
			this._buffers = buffers;
			this._bufferLock = bufferLock;
			this._autoBuffered = autoBuffered;
		}

		protected override void SpoolingWork()
		{
			TOutput toutput = default(TOutput);
			TKey tkey = default(TKey);
			int num = (this._autoBuffered ? 16 : 1);
			Pair<TKey, TOutput>[] array = new Pair<TKey, TOutput>[num];
			QueryOperatorEnumerator<TOutput, TKey> partition = this._partition;
			CancellationToken mergedCancellationToken = this._taskGroupState.CancellationState.MergedCancellationToken;
			int num2;
			do
			{
				num2 = 0;
				while (num2 < num && partition.MoveNext(ref toutput, ref tkey))
				{
					array[num2] = new Pair<TKey, TOutput>(tkey, toutput);
					num2++;
				}
				if (num2 == 0)
				{
					break;
				}
				object bufferLock = this._bufferLock;
				lock (bufferLock)
				{
					if (mergedCancellationToken.IsCancellationRequested)
					{
						break;
					}
					for (int i = 0; i < num2; i++)
					{
						this._buffers[this._partitionIndex].Enqueue(array[i]);
					}
					if (this._consumerWaiting[this._partitionIndex])
					{
						Monitor.Pulse(this._bufferLock);
						this._consumerWaiting[this._partitionIndex] = false;
					}
					if (this._buffers[this._partitionIndex].Count >= 8192)
					{
						this._producerWaiting[this._partitionIndex] = true;
						Monitor.Wait(this._bufferLock);
					}
				}
			}
			while (num2 == num);
		}

		public static void Spool(QueryTaskGroupState groupState, PartitionedStream<TOutput, TKey> partitions, bool[] consumerWaiting, bool[] producerWaiting, bool[] producerDone, Queue<Pair<TKey, TOutput>>[] buffers, object[] bufferLocks, TaskScheduler taskScheduler, bool autoBuffered)
		{
			int degreeOfParallelism = partitions.PartitionCount;
			for (int i = 0; i < degreeOfParallelism; i++)
			{
				buffers[i] = new Queue<Pair<TKey, TOutput>>(128);
				bufferLocks[i] = new object();
			}
			Task task = new Task(delegate
			{
				for (int j = 0; j < degreeOfParallelism; j++)
				{
					new OrderPreservingPipeliningSpoolingTask<TOutput, TKey>(partitions[j], groupState, consumerWaiting, producerWaiting, producerDone, j, buffers, bufferLocks[j], autoBuffered).RunAsynchronously(taskScheduler);
				}
			});
			groupState.QueryBegin(task);
			task.Start(taskScheduler);
		}

		protected override void SpoolingFinally()
		{
			object bufferLock = this._bufferLock;
			lock (bufferLock)
			{
				this._producerDone[this._partitionIndex] = true;
				if (this._consumerWaiting[this._partitionIndex])
				{
					Monitor.Pulse(this._bufferLock);
					this._consumerWaiting[this._partitionIndex] = false;
				}
			}
			base.SpoolingFinally();
			this._partition.Dispose();
		}

		private readonly QueryTaskGroupState _taskGroupState;

		private readonly QueryOperatorEnumerator<TOutput, TKey> _partition;

		private readonly bool[] _consumerWaiting;

		private readonly bool[] _producerWaiting;

		private readonly bool[] _producerDone;

		private readonly int _partitionIndex;

		private readonly Queue<Pair<TKey, TOutput>>[] _buffers;

		private readonly object _bufferLock;

		private readonly bool _autoBuffered;

		private const int PRODUCER_BUFFER_AUTO_SIZE = 16;
	}
}
