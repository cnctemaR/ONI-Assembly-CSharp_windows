using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class OrderPreservingMergeHelper<TInputOutput, TKey> : IMergeHelper<TInputOutput>
	{
		internal OrderPreservingMergeHelper(PartitionedStream<TInputOutput, TKey> partitions, TaskScheduler taskScheduler, CancellationState cancellationState, int queryId)
		{
			this._taskGroupState = new QueryTaskGroupState(cancellationState, queryId);
			this._partitions = partitions;
			this._results = new Shared<TInputOutput[]>(null);
			this._taskScheduler = taskScheduler;
		}

		void IMergeHelper<TInputOutput>.Execute()
		{
			OrderPreservingSpoolingTask<TInputOutput, TKey>.Spool(this._taskGroupState, this._partitions, this._results, this._taskScheduler);
		}

		IEnumerator<TInputOutput> IMergeHelper<TInputOutput>.GetEnumerator()
		{
			return this._results.Value.GetEnumerator();
		}

		public TInputOutput[] GetResultsAsArray()
		{
			return this._results.Value;
		}

		private QueryTaskGroupState _taskGroupState;

		private PartitionedStream<TInputOutput, TKey> _partitions;

		private Shared<TInputOutput[]> _results;

		private TaskScheduler _taskScheduler;
	}
}
