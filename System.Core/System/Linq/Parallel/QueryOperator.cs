using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal abstract class QueryOperator<TOutput> : ParallelQuery<TOutput>
	{
		internal QueryOperator(QuerySettings settings)
			: this(false, settings)
		{
		}

		internal QueryOperator(bool isOrdered, QuerySettings settings)
			: base(settings)
		{
			this._outputOrdered = isOrdered;
		}

		internal abstract QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping);

		public override IEnumerator<TOutput> GetEnumerator()
		{
			return this.GetEnumerator(null, false);
		}

		public IEnumerator<TOutput> GetEnumerator(ParallelMergeOptions? mergeOptions)
		{
			return this.GetEnumerator(mergeOptions, false);
		}

		internal bool OutputOrdered
		{
			get
			{
				return this._outputOrdered;
			}
		}

		internal virtual IEnumerator<TOutput> GetEnumerator(ParallelMergeOptions? mergeOptions, bool suppressOrderPreservation)
		{
			return new QueryOpeningEnumerator<TOutput>(this, mergeOptions, suppressOrderPreservation);
		}

		internal IEnumerator<TOutput> GetOpenedEnumerator(ParallelMergeOptions? mergeOptions, bool suppressOrder, bool forEffect, QuerySettings querySettings)
		{
			if (querySettings.ExecutionMode.Value == ParallelExecutionMode.Default && this.LimitsParallelism)
			{
				return ExceptionAggregator.WrapEnumerable<TOutput>(this.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).GetEnumerator();
			}
			QueryResults<TOutput> queryResults = this.GetQueryResults(querySettings);
			if (mergeOptions == null)
			{
				mergeOptions = querySettings.MergeOptions;
			}
			if (querySettings.CancellationState.MergedCancellationToken.IsCancellationRequested)
			{
				if (querySettings.CancellationState.ExternalCancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException(querySettings.CancellationState.ExternalCancellationToken);
				}
				throw new OperationCanceledException();
			}
			else
			{
				bool flag = this.OutputOrdered && !suppressOrder;
				PartitionedStreamMerger<TOutput> partitionedStreamMerger = new PartitionedStreamMerger<TOutput>(forEffect, mergeOptions.GetValueOrDefault(), querySettings.TaskScheduler, flag, querySettings.CancellationState, querySettings.QueryId);
				queryResults.GivePartitionedStream(partitionedStreamMerger);
				if (forEffect)
				{
					return null;
				}
				return partitionedStreamMerger.MergeExecutor.GetEnumerator();
			}
		}

		private QueryResults<TOutput> GetQueryResults(QuerySettings querySettings)
		{
			return this.Open(querySettings, false);
		}

		internal TOutput[] ExecuteAndGetResultsAsArray()
		{
			QuerySettings querySettings = base.SpecifiedQuerySettings.WithPerExecutionSettings().WithDefaults();
			QueryLifecycle.LogicalQueryExecutionBegin(querySettings.QueryId);
			TOutput[] array;
			try
			{
				if (querySettings.ExecutionMode.Value == ParallelExecutionMode.Default && this.LimitsParallelism)
				{
					array = ExceptionAggregator.WrapEnumerable<TOutput>(CancellableEnumerable.Wrap<TOutput>(this.AsSequentialQuery(querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState.ExternalCancellationToken), querySettings.CancellationState).ToArray<TOutput>();
				}
				else
				{
					QueryResults<TOutput> queryResults = this.GetQueryResults(querySettings);
					if (querySettings.CancellationState.MergedCancellationToken.IsCancellationRequested)
					{
						if (querySettings.CancellationState.ExternalCancellationToken.IsCancellationRequested)
						{
							throw new OperationCanceledException(querySettings.CancellationState.ExternalCancellationToken);
						}
						throw new OperationCanceledException();
					}
					else if (queryResults.IsIndexible && this.OutputOrdered)
					{
						ArrayMergeHelper<TOutput> arrayMergeHelper = new ArrayMergeHelper<TOutput>(base.SpecifiedQuerySettings, queryResults);
						arrayMergeHelper.Execute();
						TOutput[] resultsAsArray = arrayMergeHelper.GetResultsAsArray();
						querySettings.CleanStateAtQueryEnd();
						array = resultsAsArray;
					}
					else
					{
						PartitionedStreamMerger<TOutput> partitionedStreamMerger = new PartitionedStreamMerger<TOutput>(false, ParallelMergeOptions.FullyBuffered, querySettings.TaskScheduler, this.OutputOrdered, querySettings.CancellationState, querySettings.QueryId);
						queryResults.GivePartitionedStream(partitionedStreamMerger);
						TOutput[] resultsAsArray2 = partitionedStreamMerger.MergeExecutor.GetResultsAsArray();
						querySettings.CleanStateAtQueryEnd();
						array = resultsAsArray2;
					}
				}
			}
			finally
			{
				QueryLifecycle.LogicalQueryExecutionEnd(querySettings.QueryId);
			}
			return array;
		}

		internal abstract IEnumerable<TOutput> AsSequentialQuery(CancellationToken token);

		internal abstract bool LimitsParallelism { get; }

		internal abstract OrdinalIndexState OrdinalIndexState { get; }

		internal static ListQueryResults<TOutput> ExecuteAndCollectResults<TKey>(PartitionedStream<TOutput, TKey> openedChild, int partitionCount, bool outputOrdered, bool useStriping, QuerySettings settings)
		{
			TaskScheduler taskScheduler = settings.TaskScheduler;
			return new ListQueryResults<TOutput>(MergeExecutor<TOutput>.Execute<TKey>(openedChild, false, ParallelMergeOptions.FullyBuffered, taskScheduler, outputOrdered, settings.CancellationState, settings.QueryId).GetResultsAsArray(), partitionCount, useStriping);
		}

		internal static QueryOperator<TOutput> AsQueryOperator(IEnumerable<TOutput> source)
		{
			QueryOperator<TOutput> queryOperator = source as QueryOperator<TOutput>;
			if (queryOperator == null)
			{
				OrderedParallelQuery<TOutput> orderedParallelQuery = source as OrderedParallelQuery<TOutput>;
				if (orderedParallelQuery != null)
				{
					queryOperator = orderedParallelQuery.SortOperator;
				}
				else
				{
					queryOperator = new ScanQueryOperator<TOutput>(source);
				}
			}
			return queryOperator;
		}

		protected bool _outputOrdered;
	}
}
