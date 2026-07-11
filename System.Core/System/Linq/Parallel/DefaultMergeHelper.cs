using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class DefaultMergeHelper<TInputOutput, TIgnoreKey> : IMergeHelper<TInputOutput>
	{
		internal DefaultMergeHelper(PartitionedStream<TInputOutput, TIgnoreKey> partitions, bool ignoreOutput, ParallelMergeOptions options, TaskScheduler taskScheduler, CancellationState cancellationState, int queryId)
		{
			this._taskGroupState = new QueryTaskGroupState(cancellationState, queryId);
			this._partitions = partitions;
			this._taskScheduler = taskScheduler;
			this._ignoreOutput = ignoreOutput;
			IntValueEvent intValueEvent = new IntValueEvent();
			if (!ignoreOutput)
			{
				if (options != ParallelMergeOptions.FullyBuffered)
				{
					if (partitions.PartitionCount > 1)
					{
						this._asyncChannels = MergeExecutor<TInputOutput>.MakeAsynchronousChannels(partitions.PartitionCount, options, intValueEvent, cancellationState.MergedCancellationToken);
						this._channelEnumerator = new AsynchronousChannelMergeEnumerator<TInputOutput>(this._taskGroupState, this._asyncChannels, intValueEvent);
						return;
					}
					this._channelEnumerator = ExceptionAggregator.WrapQueryEnumerator<TInputOutput, TIgnoreKey>(partitions[0], this._taskGroupState.CancellationState).GetEnumerator();
					return;
				}
				else
				{
					this._syncChannels = MergeExecutor<TInputOutput>.MakeSynchronousChannels(partitions.PartitionCount);
					this._channelEnumerator = new SynchronousChannelMergeEnumerator<TInputOutput>(this._taskGroupState, this._syncChannels);
				}
			}
		}

		void IMergeHelper<TInputOutput>.Execute()
		{
			if (this._asyncChannels != null)
			{
				SpoolingTask.SpoolPipeline<TInputOutput, TIgnoreKey>(this._taskGroupState, this._partitions, this._asyncChannels, this._taskScheduler);
				return;
			}
			if (this._syncChannels != null)
			{
				SpoolingTask.SpoolStopAndGo<TInputOutput, TIgnoreKey>(this._taskGroupState, this._partitions, this._syncChannels, this._taskScheduler);
				return;
			}
			if (this._ignoreOutput)
			{
				SpoolingTask.SpoolForAll<TInputOutput, TIgnoreKey>(this._taskGroupState, this._partitions, this._taskScheduler);
			}
		}

		IEnumerator<TInputOutput> IMergeHelper<TInputOutput>.GetEnumerator()
		{
			return this._channelEnumerator;
		}

		public TInputOutput[] GetResultsAsArray()
		{
			if (this._syncChannels != null)
			{
				int num = 0;
				for (int i = 0; i < this._syncChannels.Length; i++)
				{
					num += this._syncChannels[i].Count;
				}
				TInputOutput[] array = new TInputOutput[num];
				int num2 = 0;
				for (int j = 0; j < this._syncChannels.Length; j++)
				{
					this._syncChannels[j].CopyTo(array, num2);
					num2 += this._syncChannels[j].Count;
				}
				return array;
			}
			List<TInputOutput> list = new List<TInputOutput>();
			foreach (TInputOutput tinputOutput in ((IMergeHelper<TInputOutput>)this))
			{
				list.Add(tinputOutput);
			}
			return list.ToArray();
		}

		private QueryTaskGroupState _taskGroupState;

		private PartitionedStream<TInputOutput, TIgnoreKey> _partitions;

		private AsynchronousChannel<TInputOutput>[] _asyncChannels;

		private SynchronousChannel<TInputOutput>[] _syncChannels;

		private IEnumerator<TInputOutput> _channelEnumerator;

		private TaskScheduler _taskScheduler;

		private bool _ignoreOutput;
	}
}
