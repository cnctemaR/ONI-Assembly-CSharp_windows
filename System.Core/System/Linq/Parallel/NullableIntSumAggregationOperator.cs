using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableIntSumAggregationOperator : InlinedAggregationOperator<int?, int?, int?>
	{
		internal NullableIntSumAggregationOperator(IEnumerable<int?> child)
			: base(child)
		{
		}

		protected override int? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			int? num3;
			using (IEnumerator<int?> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				int num = 0;
				while (enumerator.MoveNext())
				{
					int num2 = num;
					num3 = enumerator.Current;
					num = checked(num2 + num3.GetValueOrDefault());
				}
				num3 = new int?(num);
			}
			return num3;
		}

		protected override QueryOperatorEnumerator<int?, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<int?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableIntSumAggregationOperator.NullableIntSumAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableIntSumAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<int?>
		{
			internal NullableIntSumAggregationOperatorEnumerator(QueryOperatorEnumerator<int?, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref int? currentElement)
			{
				int? num = null;
				TKey tkey = default(TKey);
				QueryOperatorEnumerator<int?, TKey> source = this._source;
				if (source.MoveNext(ref num, ref tkey))
				{
					int num2 = 0;
					int num3 = 0;
					do
					{
						if ((num3++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						checked
						{
							num2 += num.GetValueOrDefault();
						}
					}
					while (source.MoveNext(ref num, ref tkey));
					currentElement = new int?(num2);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<int?, TKey> _source;
		}
	}
}
