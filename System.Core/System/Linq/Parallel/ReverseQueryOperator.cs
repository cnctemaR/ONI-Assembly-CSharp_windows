using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ReverseQueryOperator<TSource> : UnaryQueryOperator<TSource, TSource>
	{
		internal ReverseQueryOperator(IEnumerable<TSource> child)
			: base(child)
		{
			if (base.Child.OrdinalIndexState == OrdinalIndexState.Indexable)
			{
				base.SetOrdinalIndexState(OrdinalIndexState.Indexable);
				return;
			}
			base.SetOrdinalIndexState(OrdinalIndexState.Shuffled);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TSource, TKey> partitionedStream = new PartitionedStream<TSource, TKey>(partitionCount, new ReverseComparer<TKey>(inputStream.KeyComparer), OrdinalIndexState.Shuffled);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new ReverseQueryOperator<TSource>.ReverseQueryOperatorEnumerator<TKey>(inputStream[i], settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return ReverseQueryOperator<TSource>.ReverseQueryOperatorResults.NewResults(base.Child.Open(settings, false), this, settings, preferStriping);
		}

		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			return CancellableEnumerable.Wrap<TSource>(base.Child.AsSequentialQuery(token), token).Reverse<TSource>();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private class ReverseQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TSource, TKey>
		{
			internal ReverseQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, TKey> source, CancellationToken cancellationToken)
			{
				this._source = source;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TSource currentElement, ref TKey currentKey)
			{
				if (this._buffer == null)
				{
					this._bufferIndex = new Shared<int>(0);
					this._buffer = new List<Pair<TSource, TKey>>();
					TSource tsource = default(TSource);
					TKey tkey = default(TKey);
					int num = 0;
					while (this._source.MoveNext(ref tsource, ref tkey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						this._buffer.Add(new Pair<TSource, TKey>(tsource, tkey));
						this._bufferIndex.Value++;
					}
				}
				Shared<int> bufferIndex = this._bufferIndex;
				int num2 = bufferIndex.Value - 1;
				bufferIndex.Value = num2;
				if (num2 >= 0)
				{
					currentElement = this._buffer[this._bufferIndex.Value].First;
					currentKey = this._buffer[this._bufferIndex.Value].Second;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TSource, TKey> _source;

			private readonly CancellationToken _cancellationToken;

			private List<Pair<TSource, TKey>> _buffer;

			private Shared<int> _bufferIndex;
		}

		private class ReverseQueryOperatorResults : UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults
		{
			public static QueryResults<TSource> NewResults(QueryResults<TSource> childQueryResults, ReverseQueryOperator<TSource> op, QuerySettings settings, bool preferStriping)
			{
				if (childQueryResults.IsIndexible)
				{
					return new ReverseQueryOperator<TSource>.ReverseQueryOperatorResults(childQueryResults, op, settings, preferStriping);
				}
				return new UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults(childQueryResults, op, settings, preferStriping);
			}

			private ReverseQueryOperatorResults(QueryResults<TSource> childQueryResults, ReverseQueryOperator<TSource> op, QuerySettings settings, bool preferStriping)
				: base(childQueryResults, op, settings, preferStriping)
			{
				this._count = this._childQueryResults.ElementsCount;
			}

			internal override bool IsIndexible
			{
				get
				{
					return true;
				}
			}

			internal override int ElementsCount
			{
				get
				{
					return this._count;
				}
			}

			internal override TSource GetElement(int index)
			{
				return this._childQueryResults.GetElement(this._count - index - 1);
			}

			private int _count;
		}
	}
}
