using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal abstract class InlinedAggregationOperator<TSource, TIntermediate, TResult> : UnaryQueryOperator<TSource, TIntermediate>
	{
		internal InlinedAggregationOperator(IEnumerable<TSource> child)
			: base(child)
		{
		}

		internal TResult Aggregate()
		{
			Exception ex = null;
			TResult tresult;
			try
			{
				tresult = this.InternalAggregate(ref ex);
			}
			catch (Exception ex2)
			{
				if (ex2 is AggregateException)
				{
					throw;
				}
				OperationCanceledException ex3 = ex2 as OperationCanceledException;
				if (ex3 != null && ex3.CancellationToken == base.SpecifiedQuerySettings.CancellationState.ExternalCancellationToken && base.SpecifiedQuerySettings.CancellationState.ExternalCancellationToken.IsCancellationRequested)
				{
					throw;
				}
				throw new AggregateException(new Exception[] { ex2 });
			}
			if (ex != null)
			{
				throw ex;
			}
			return tresult;
		}

		protected abstract TResult InternalAggregate(ref Exception singularExceptionToThrow);

		internal override QueryResults<TIntermediate> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, TIntermediate>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TIntermediate> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TIntermediate, int> partitionedStream = new PartitionedStream<TIntermediate, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = this.CreateEnumerator<TKey>(i, partitionCount, inputStream[i], null, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<int>(partitionedStream);
		}

		protected abstract QueryOperatorEnumerator<TIntermediate, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<TSource, TKey> source, object sharedData, CancellationToken cancellationToken);

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TIntermediate> AsSequentialQuery(CancellationToken token)
		{
			throw new NotSupportedException();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}
	}
}
