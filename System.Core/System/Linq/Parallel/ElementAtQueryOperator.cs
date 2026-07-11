using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ElementAtQueryOperator<TSource> : UnaryQueryOperator<TSource, TSource>
	{
		internal ElementAtQueryOperator(IEnumerable<TSource> child, int index)
			: base(child)
		{
			this._index = index;
			OrdinalIndexState ordinalIndexState = base.Child.OrdinalIndexState;
			if (ordinalIndexState.IsWorseThan(OrdinalIndexState.Correct))
			{
				this._prematureMerge = true;
				this._limitsParallelism = ordinalIndexState != OrdinalIndexState.Shuffled;
			}
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults(base.Child.Open(settings, false), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TSource, int> partitionedStream;
			if (this._prematureMerge)
			{
				partitionedStream = QueryOperator<TSource>.ExecuteAndCollectResults<TKey>(inputStream, partitionCount, base.Child.OutputOrdered, preferStriping, settings).GetPartitionedStream();
			}
			else
			{
				partitionedStream = (PartitionedStream<TSource, int>)inputStream;
			}
			Shared<bool> shared = new Shared<bool>(false);
			PartitionedStream<TSource, int> partitionedStream2 = new PartitionedStream<TSource, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream2[i] = new ElementAtQueryOperator<TSource>.ElementAtQueryOperatorEnumerator(partitionedStream[i], this._index, shared, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<int>(partitionedStream2);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			throw new NotSupportedException();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._limitsParallelism;
			}
		}

		internal bool Aggregate(out TSource result, bool withDefaultValue)
		{
			if (this.LimitsParallelism && base.SpecifiedQuerySettings.WithDefaults().ExecutionMode.Value != ParallelExecutionMode.ForceParallelism)
			{
				CancellationState cancellationState = base.SpecifiedQuerySettings.CancellationState;
				if (withDefaultValue)
				{
					IEnumerable<TSource> enumerable = CancellableEnumerable.Wrap<TSource>(base.Child.AsSequentialQuery(cancellationState.ExternalCancellationToken), cancellationState.ExternalCancellationToken);
					result = ExceptionAggregator.WrapEnumerable<TSource>(enumerable, cancellationState).ElementAtOrDefault<TSource>(this._index);
				}
				else
				{
					IEnumerable<TSource> enumerable2 = CancellableEnumerable.Wrap<TSource>(base.Child.AsSequentialQuery(cancellationState.ExternalCancellationToken), cancellationState.ExternalCancellationToken);
					result = ExceptionAggregator.WrapEnumerable<TSource>(enumerable2, cancellationState).ElementAt<TSource>(this._index);
				}
				return true;
			}
			using (IEnumerator<TSource> enumerator = base.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered)))
			{
				if (enumerator.MoveNext())
				{
					TSource tsource = enumerator.Current;
					result = tsource;
					return true;
				}
			}
			result = default(TSource);
			return false;
		}

		private readonly int _index;

		private readonly bool _prematureMerge;

		private readonly bool _limitsParallelism;

		private class ElementAtQueryOperatorEnumerator : QueryOperatorEnumerator<TSource, int>
		{
			internal ElementAtQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, int> source, int index, Shared<bool> resultFoundFlag, CancellationToken cancellationToken)
			{
				this._source = source;
				this._index = index;
				this._resultFoundFlag = resultFoundFlag;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TSource currentElement, ref int currentKey)
			{
				int num = 0;
				while (this._source.MoveNext(ref currentElement, ref currentKey))
				{
					if ((num++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					if (this._resultFoundFlag.Value)
					{
						break;
					}
					if (currentKey == this._index)
					{
						this._resultFoundFlag.Value = true;
						return true;
					}
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<TSource, int> _source;

			private int _index;

			private Shared<bool> _resultFoundFlag;

			private CancellationToken _cancellationToken;
		}
	}
}
