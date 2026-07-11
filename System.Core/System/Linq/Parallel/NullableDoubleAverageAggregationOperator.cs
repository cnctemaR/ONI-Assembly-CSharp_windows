using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableDoubleAverageAggregationOperator : InlinedAggregationOperator<double?, Pair<double, long>, double?>
	{
		internal NullableDoubleAverageAggregationOperator(IEnumerable<double?> child)
			: base(child)
		{
		}

		protected override double? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			double? num;
			using (IEnumerator<Pair<double, long>> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					num = null;
					num = num;
				}
				else
				{
					Pair<double, long> pair = enumerator.Current;
					while (enumerator.MoveNext())
					{
						double first = pair.First;
						Pair<double, long> pair2 = enumerator.Current;
						pair.First = first + pair2.First;
						long second = pair.Second;
						pair2 = enumerator.Current;
						pair.Second = checked(second + pair2.Second);
					}
					num = new double?(pair.First / (double)pair.Second);
				}
			}
			return num;
		}

		protected override QueryOperatorEnumerator<Pair<double, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<double?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableDoubleAverageAggregationOperator.NullableDoubleAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableDoubleAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<double, long>>
		{
			internal NullableDoubleAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<double?, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<double, long> currentElement)
			{
				double num = 0.0;
				long num2 = 0L;
				QueryOperatorEnumerator<double?, TKey> source = this._source;
				double? num3 = null;
				TKey tkey = default(TKey);
				int num4 = 0;
				while (source.MoveNext(ref num3, ref tkey))
				{
					if (num3 != null)
					{
						if ((num4++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						num += num3.GetValueOrDefault();
						checked
						{
							num2 += 1L;
						}
					}
				}
				currentElement = new Pair<double, long>(num, num2);
				return num2 > 0L;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<double?, TKey> _source;
		}
	}
}
