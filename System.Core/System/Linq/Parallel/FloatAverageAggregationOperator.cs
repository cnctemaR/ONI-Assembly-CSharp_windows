using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class FloatAverageAggregationOperator : InlinedAggregationOperator<float, Pair<double, long>, float>
	{
		internal FloatAverageAggregationOperator(IEnumerable<float> child)
			: base(child)
		{
		}

		protected override float InternalAggregate(ref Exception singularExceptionToThrow)
		{
			float num;
			using (IEnumerator<Pair<double, long>> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					num = 0f;
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
					num = (float)(pair.First / (double)pair.Second);
				}
			}
			return num;
		}

		protected override QueryOperatorEnumerator<Pair<double, long>, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<float, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new FloatAverageAggregationOperator.FloatAverageAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class FloatAverageAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<Pair<double, long>>
		{
			internal FloatAverageAggregationOperatorEnumerator(QueryOperatorEnumerator<float, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref Pair<double, long> currentElement)
			{
				double num = 0.0;
				long num2 = 0L;
				QueryOperatorEnumerator<float, TKey> source = this._source;
				float num3 = 0f;
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
						num += (double)num3;
						checked
						{
							num2 += 1L;
						}
					}
					while (source.MoveNext(ref num3, ref tkey));
					currentElement = new Pair<double, long>(num, num2);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<float, TKey> _source;
		}
	}
}
