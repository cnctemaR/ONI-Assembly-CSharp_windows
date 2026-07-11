using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class LongAverageAggregationOperator : InlinedAggregationOperator<long, Pair<long, long>, double>
	{
		internal LongAverageAggregationOperator(IEnumerable<long> child)
			: base(child)
		{
		}

		protected override double InternalAggregate(ref Exception singularExceptionToThrow)
		{
			checked
			{
				double num;
				using (IEnumerator<Pair<long, long>> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
				{
					if (!enumerator.MoveNext())
					{
						singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
						num = 0.0;
					}
					else
					{
						Pair<long, long> pair = enumerator.Current;
						while (enumerator.MoveNext())
						{
							long first = pair.First;
							Pair<long, long> pair2 = enumerator.Current;
							pair.First = first + pair2.First;
							long second = pair.Second;
							pair2 = enumerator.Current;
							pair.Second = second + pair2.Second;
						}
						num = (double)pair.First / (double)pair.Second;
					}
				}
				return num;
			}
		}

		protected override QueryOperatorEnumerator<Pair<long, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<long, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new LongAverageAggregationOperator.LongAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class LongAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<long, long>>
		{
			internal LongAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<long, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<long, long> currentElement)
			{
				long num = 0L;
				long num2 = 0L;
				QueryOperatorEnumerator<long, TKey> source = this._source;
				long num3 = 0L;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref num3, ref tkey))
				{
					int num4 = 0;
					do
					{
						if ((num4++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						checked
						{
							num += num3;
							num2 += 1L;
						}
					}
					while (source.MoveNext(ref num3, ref tkey));
					currentElement = new Pair<long, long>(num, num2);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<long, TKey> _source;
		}
	}
}
