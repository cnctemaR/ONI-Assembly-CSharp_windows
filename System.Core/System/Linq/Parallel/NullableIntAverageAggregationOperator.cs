using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableIntAverageAggregationOperator : InlinedAggregationOperator<int?, Pair<long, long>, double?>
	{
		internal NullableIntAverageAggregationOperator(IEnumerable<int?> child)
			: base(child)
		{
		}

		protected override double? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			checked
			{
				double? num;
				using (IEnumerator<Pair<long, long>> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
				{
					if (!enumerator.MoveNext())
					{
						num = null;
						num = num;
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
						num = new double?((double)pair.First / (double)pair.Second);
					}
				}
				return num;
			}
		}

		protected override QueryOperatorEnumerator<Pair<long, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<int?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableIntAverageAggregationOperator.NullableIntAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableIntAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<long, long>>
		{
			internal NullableIntAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<int?, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<long, long> currentElement)
			{
				long num = 0L;
				long num2 = 0L;
				QueryOperatorEnumerator<int?, TKey> source = this._source;
				int? num3 = null;
				TKey tkey = default(TKey);
				int num4 = 0;
				while (source.MoveNext(ref num3, ref tkey))
				{
					if ((num4++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					checked
					{
						if (num3 != null)
						{
							num += unchecked((long)num3.GetValueOrDefault());
							num2 += 1L;
						}
					}
				}
				currentElement = new Pair<long, long>(num, num2);
				return num2 > 0L;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<int?, TKey> _source;
		}
	}
}
