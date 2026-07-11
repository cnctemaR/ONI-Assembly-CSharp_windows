using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class FloatMinMaxAggregationOperator : InlinedAggregationOperator<float, float, float>
	{
		internal FloatMinMaxAggregationOperator(IEnumerable<float> child, int sign)
			: base(child)
		{
			this._sign = sign;
		}

		protected override float InternalAggregate(ref Exception singularExceptionToThrow)
		{
			float num;
			using (IEnumerator<float> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					num = 0f;
				}
				else
				{
					float num2 = enumerator.Current;
					if (this._sign == -1)
					{
						while (enumerator.MoveNext())
						{
							float num3 = enumerator.Current;
							if (num3 < num2 || float.IsNaN(num3))
							{
								num2 = num3;
							}
						}
					}
					else
					{
						while (enumerator.MoveNext())
						{
							float num4 = enumerator.Current;
							if (num4 > num2 || float.IsNaN(num2))
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

		protected override QueryOperatorEnumerator<float, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<float, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new FloatMinMaxAggregationOperator.FloatMinMaxAggregationOperatorEnumerator<TKey>(source, index, this._sign, cancellationToken);
		}

		private readonly int _sign;

		private class FloatMinMaxAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<float>
		{
			internal FloatMinMaxAggregationOperatorEnumerator(QueryOperatorEnumerator<float, TKey> source, int partitionIndex, int sign, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
				this._sign = sign;
			}

			protected override bool MoveNextCore(ref float currentElement)
			{
				QueryOperatorEnumerator<float, TKey> source = this._source;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref currentElement, ref tkey))
				{
					int num = 0;
					if (this._sign == -1)
					{
						float num2 = 0f;
						while (source.MoveNext(ref num2, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num2 < currentElement || float.IsNaN(num2))
							{
								currentElement = num2;
							}
						}
					}
					else
					{
						float num3 = 0f;
						while (source.MoveNext(ref num3, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num3 > currentElement || float.IsNaN(currentElement))
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

			private QueryOperatorEnumerator<float, TKey> _source;

			private int _sign;
		}
	}
}
