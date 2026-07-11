using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableFloatAverageAggregationOperator : InlinedAggregationOperator<float?, Pair<double, long>, float?>
	{
		internal NullableFloatAverageAggregationOperator(IEnumerable<float?> child)
			: base(child)
		{
		}

		protected override float? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			float? num;
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
					num = new float?((float)(pair.First / (double)pair.Second));
				}
			}
			return num;
		}

		protected override QueryOperatorEnumerator<Pair<double, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<float?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableFloatAverageAggregationOperator.NullableFloatAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableFloatAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<double, long>>
		{
			internal NullableFloatAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<float?, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<double, long> currentElement)
			{
				double num = 0.0;
				long num2 = 0L;
				QueryOperatorEnumerator<float?, TKey> source = this._source;
				float? num3 = null;
				TKey tkey = default(TKey);
				int num4 = 0;
				while (source.MoveNext(ref num3, ref tkey))
				{
					if ((num4++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					if (num3 != null)
					{
						num += (double)num3.GetValueOrDefault();
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

			private QueryOperatorEnumerator<float?, TKey> _source;
		}
	}
}
