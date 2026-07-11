using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class IntMinMaxAggregationOperator : InlinedAggregationOperator<int, int, int>
	{
		internal IntMinMaxAggregationOperator(IEnumerable<int> child, int sign)
			: base(child)
		{
			this._sign = sign;
		}

		protected override int InternalAggregate(ref Exception singularExceptionToThrow)
		{
			int num;
			using (IEnumerator<int> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					num = 0;
				}
				else
				{
					int num2 = enumerator.Current;
					if (this._sign == -1)
					{
						while (enumerator.MoveNext())
						{
							int num3 = enumerator.Current;
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
							int num4 = enumerator.Current;
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

		protected override QueryOperatorEnumerator<int, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<int, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new IntMinMaxAggregationOperator.IntMinMaxAggregationOperatorEnumerator<TKey>(source, index, this._sign, cancellationToken);
		}

		private readonly int _sign;

		private class IntMinMaxAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<int>
		{
			internal IntMinMaxAggregationOperatorEnumerator(QueryOperatorEnumerator<int, TKey> source, int partitionIndex, int sign, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
				this._sign = sign;
			}

			protected override bool MoveNextCore(ref int currentElement)
			{
				QueryOperatorEnumerator<int, TKey> source = this._source;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref currentElement, ref tkey))
				{
					int num = 0;
					if (this._sign == -1)
					{
						int num2 = 0;
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
						int num3 = 0;
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

			private readonly QueryOperatorEnumerator<int, TKey> _source;

			private readonly int _sign;
		}
	}
}
