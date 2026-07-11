using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class NullableLongSumAggregationOperator : InlinedAggregationOperator<long?, long?, long?>
	{
		internal NullableLongSumAggregationOperator(IEnumerable<long?> child)
			: base(child)
		{
		}

		protected override long? InternalAggregate(ref Exception singularExceptionToThrow)
		{
			long? num3;
			using (IEnumerator<long?> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				long num = 0L;
				while (enumerator.MoveNext())
				{
					long num2 = num;
					num3 = enumerator.Current;
					num = checked(num2 + num3.GetValueOrDefault());
				}
				num3 = new long?(num);
			}
			return num3;
		}

		protected override QueryOperatorEnumerator<long?, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<long?, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new NullableLongSumAggregationOperator.NullableLongSumAggregationOperatorEnumerator<TKey>(source, index, cancellationToken);
		}

		private class NullableLongSumAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<long?>
		{
			internal NullableLongSumAggregationOperatorEnumerator(QueryOperatorEnumerator<long?, TKey> source, int partitionIndex, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
			}

			protected override bool MoveNextCore(ref long? currentElement)
			{
				long? num = null;
				TKey tkey = default(TKey);
				QueryOperatorEnumerator<long?, TKey> source = this._source;
				if (source.MoveNext(ref num, ref tkey))
				{
					long num2 = 0L;
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
					currentElement = new long?(num2);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<long?, TKey> _source;
		}
	}
}
