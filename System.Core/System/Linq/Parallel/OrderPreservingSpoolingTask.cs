using System;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class OrderPreservingSpoolingTask<TInputOutput, TKey> : SpoolingTaskBase
	{
		private OrderPreservingSpoolingTask(int taskIndex, QueryTaskGroupState groupState, Shared<TInputOutput[]> results, SortHelper<TInputOutput> sortHelper)
			: base(taskIndex, groupState)
		{
			this._results = results;
			this._sortHelper = sortHelper;
		}

		internal static void Spool(QueryTaskGroupState groupState, PartitionedStream<TInputOutput, TKey> partitions, Shared<TInputOutput[]> results, TaskScheduler taskScheduler)
		{
			int maxToRunInParallel = partitions.PartitionCount - 1;
			SortHelper<TInputOutput, TKey>[] sortHelpers = SortHelper<TInputOutput, TKey>.GenerateSortHelpers(partitions, groupState);
			Task task = new Task(delegate
			{
				for (int j = 0; j < maxToRunInParallel; j++)
				{
					new OrderPreservingSpoolingTask<TInputOutput, TKey>(j, groupState, results, sortHelpers[j]).RunAsynchronously(taskScheduler);
				}
				new OrderPreservingSpoolingTask<TInputOutput, TKey>(maxToRunInParallel, groupState, results, sortHelpers[maxToRunInParallel]).RunSynchronously(taskScheduler);
			});
			groupState.QueryBegin(task);
			task.RunSynchronously(taskScheduler);
			for (int i = 0; i < sortHelpers.Length; i++)
			{
				sortHelpers[i].Dispose();
			}
			groupState.QueryEnd(false);
		}

		protected override void SpoolingWork()
		{
			TInputOutput[] array = this._sortHelper.Sort();
			if (!this._groupState.CancellationState.MergedCancellationToken.IsCancellationRequested && this._taskIndex == 0)
			{
				this._results.Value = array;
			}
		}

		private Shared<TInputOutput[]> _results;

		private SortHelper<TInputOutput> _sortHelper;
	}
}
