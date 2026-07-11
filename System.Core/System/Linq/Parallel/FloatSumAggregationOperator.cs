using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class FloatSumAggregationOperator : InlinedAggregationOperator<float, double, float>
	{
		internal FloatSumAggregationOperator(IEnumerable<float> child)
			: base(child)
		{
		}

		protected override float InternalAggregate(ref Exception singularExceptionToThrow)
		{
			float num3;
			using (IEnumerator<double> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				double num = 0.0;
				while (enumerator.MoveNext())
				{
					double num2 = enumerator.Current;
					num += num2;
				}
				num3 = (float)num;
			}
			return num3;
		}

		protected override QueryOperatorEnumerator<double, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<float, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new FloatSumAggregationOperator.FloatSumAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class FloatSumAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<double>
		{
			internal FloatSumAggregationOperatorEnumerator(QueryOperatorEnumerator<float, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref double currentElement)
			{
				float num = 0f;
				TKey tkey = default(TKey);
				QueryOperatorEnumerator<float, TKey> source = this._source;
				if (source.MoveNext(ref num, ref tkey))
				{
					double num2 = 0.0;
					int num3 = 0;
					do
					{
						if ((num3++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						num2 += (double)num;
					}
					while (source.MoveNext(ref num, ref tkey));
					currentElement = num2;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<float, TKey> _source;
		}
	}
}
