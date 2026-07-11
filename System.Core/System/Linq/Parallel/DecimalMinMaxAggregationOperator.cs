using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DecimalMinMaxAggregationOperator : InlinedAggregationOperator<decimal, decimal, decimal>
	{
		internal DecimalMinMaxAggregationOperator(IEnumerable<decimal> child, int sign)
			: base(child)
		{
			this._sign = sign;
		}

		protected override decimal InternalAggregate(ref Exception singularExceptionToThrow)
		{
			decimal num;
			using (IEnumerator<decimal> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					num = 0m;
				}
				else
				{
					decimal num2 = enumerator.Current;
					if (this._sign == -1)
					{
						while (enumerator.MoveNext())
						{
							decimal num3 = enumerator.Current;
							if (num3 < num2)
							{
								num2 = num3;
							}
						}
					}
					else
					{
						while (enumerator.MoveNext())
						{
							decimal num4 = enumerator.Current;
							if (num4 > num2)
							{
								num2 = num4;
							}
						}
					}
					num = num2;
				}
			}
			return num;
		}

		protected override QueryOperatorEnumerator<decimal, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<decimal, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new DecimalMinMaxAggregationOperator.DecimalMinMaxAggregationOperatorEnumerator<TKey>(source, index, this._sign, cancellationToken);
		}

		private readonly int _sign;

		private class DecimalMinMaxAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<decimal>
		{
			internal DecimalMinMaxAggregationOperatorEnumerator(QueryOperatorEnumerator<decimal, TKey> source, int partitionIndex, int sign, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
				this._sign = sign;
			}

			protected override bool MoveNextCore(ref decimal currentElement)
			{
				QueryOperatorEnumerator<decimal, TKey> source = this._source;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref currentElement, ref tkey))
				{
					int num = 0;
					if (this._sign == -1)
					{
						decimal num2 = 0m;
						while (source.MoveNext(ref num2, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num2 < currentElement)
							{
								currentElement = num2;
							}
						}
					}
					else
					{
						decimal num3 = 0m;
						while (source.MoveNext(ref num3, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num3 > currentElement)
							{
								currentElement = num3;
							}
						}
					}
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<decimal, TKey> _source;

			private int _sign;
		}
	}
}
