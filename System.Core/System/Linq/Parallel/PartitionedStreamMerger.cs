using System;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class PartitionedStreamMerger<TOutput> : IPartitionedStreamRecipient<TOutput>
	{
		internal MergeExecutor<TOutput> MergeExecutor
		{
			get
			{
				return this._mergeExecutor;
			}
		}

		internal PartitionedStreamMerger(bool forEffectMerge, ParallelMergeOptions mergeOptions, TaskScheduler taskScheduler, bool outputOrdered, CancellationState cancellationState, int queryId)
		{
			this._forEffectMerge = forEffectMerge;
			this._mergeOptions = mergeOptions;
			this._isOrdered = outputOrdered;
			this._taskScheduler = taskScheduler;
			this._cancellationState = cancellationState;
			this._queryId = queryId;
		}

		public void Receive<TKey>(PartitionedStream<TOutput, TKey> partitionedStream)
		{
			this._mergeExecutor = MergeExecutor<TOutput>.Execute<TKey>(partitionedStream, this._forEffectMerge, this._mergeOptions, this._taskScheduler, this._isOrdered, this._cancellationState, this._queryId);
		}

		private bool _forEffectMerge;

		private ParallelMergeOptions _mergeOptions;

		private bool _isOrdered;

		private MergeExecutor<TOutput> _mergeExecutor;

		private TaskScheduler _taskScheduler;

		private int _queryId;

		private CancellationState _cancellationState;
	}
}
