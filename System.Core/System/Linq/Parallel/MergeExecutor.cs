using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class MergeExecutor<TInputOutput> : IEnumerable<TInputOutput>, IEnumerable
	{
		private MergeExecutor()
		{
		}

		internal static MergeExecutor<TInputOutput> Execute<TKey>(PartitionedStream<TInputOutput, TKey> partitions, bool ignoreOutput, ParallelMergeOptions options, TaskScheduler taskScheduler, bool isOrdered, CancellationState cancellationState, int queryId)
		{
			MergeExecutor<TInputOutput> mergeExecutor = new MergeExecutor<TInputOutput>();
			if (isOrdered && !ignoreOutput)
			{
				if (options != ParallelMergeOptions.FullyBuffered && !partitions.OrdinalIndexState.IsWorseThan(OrdinalIndexState.Increasing))
				{
					bool flag = options == ParallelMergeOptions.AutoBuffered;
					if (partitions.PartitionCount > 1)
					{
						mergeExecutor._mergeHelper = new OrderPreservingPipeliningMergeHelper<TInputOutput, TKey>(partitions, taskScheduler, cancellationState, flag, queryId, partitions.KeyComparer);
					}
					else
					{
						mergeExecutor._mergeHelper = new DefaultMergeHelper<TInputOutput, TKey>(partitions, false, options, taskScheduler, cancellationState, queryId);
					}
				}
				else
				{
					mergeExecutor._mergeHelper = new OrderPreservingMergeHelper<TInputOutput, TKey>(partitions, taskScheduler, cancellationState, queryId);
				}
			}
			else
			{
				mergeExecutor._mergeHelper = new DefaultMergeHelper<TInputOutput, TKey>(partitions, ignoreOutput, options, taskScheduler, cancellationState, queryId);
			}
			mergeExecutor.Execute();
			return mergeExecutor;
		}

		private void Execute()
		{
			this._mergeHelper.Execute();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<TInputOutput> GetEnumerator()
		{
			return this._mergeHelper.GetEnumerator();
		}

		internal TInputOutput[] GetResultsAsArray()
		{
			return this._mergeHelper.GetResultsAsArray();
		}

		internal static AsynchronousChannel<TInputOutput>[] MakeAsynchronousChannels(int partitionCount, ParallelMergeOptions options, IntValueEvent consumerEvent, CancellationToken cancellationToken)
		{
			AsynchronousChannel<TInputOutput>[] array = new AsynchronousChannel<TInputOutput>[partitionCount];
			int num = 0;
			if (options == ParallelMergeOptions.NotBuffered)
			{
				num = 1;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new AsynchronousChannel<TInputOutput>(i, num, cancellationToken, consumerEvent);
			}
			return array;
		}

		internal static SynchronousChannel<TInputOutput>[] MakeSynchronousChannels(int partitionCount)
		{
			SynchronousChannel<TInputOutput>[] array = new SynchronousChannel<TInputOutput>[partitionCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SynchronousChannel<TInputOutput>();
			}
			return array;
		}

		private IMergeHelper<TInputOutput> _mergeHelper;
	}
}
