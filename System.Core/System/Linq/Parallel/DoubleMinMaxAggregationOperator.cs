using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DoubleMinMaxAggregationOperator : InlinedAggregationOperator<double, double, double>
	{
		internal DoubleMinMaxAggregationOperator(IEnumerable<double> child, int sign)
			: base(child)
		{
			this._sign = sign;
		}

		protected override double InternalAggregate(ref Exception singularExceptionToThrow)
		{
			double num;
			using (IEnumerator<double> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				if (!enumerator.MoveNext())
				{
					singularExceptionToThrow = new InvalidOperationException("Sequence contains no elements");
					num = 0.0;
				}
				else
				{
					double num2 = enumerator.Current;
					if (this._sign == -1)
					{
						while (enumerator.MoveNext())
						{
							double num3 = enumerator.Current;
							if (num3 < num2 || double.IsNaN(num3))
							{
								num2 = num3;
							}
						}
					}
					else
					{
						while (enumerator.MoveNext())
						{
							double num4 = enumerator.Current;
							if (num4 > num2 || double.IsNaN(num2))
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

		protected override QueryOperatorEnumerator<double, int> CreateEnumerator<TKey>(int index, int count, QueryOperatorEnumerator<double, TKey> source, object sharedData, CancellationToken cancellationToken)
		{
			return new DoubleMinMaxAggregationOperator.DoubleMinMaxAggregationOperatorEnumerator<TKey>(source, index, this._sign, cancellationToken);
		}

		private readonly int _sign;

		private class DoubleMinMaxAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<double>
		{
			internal DoubleMinMaxAggregationOperatorEnumerator(QueryOperatorEnumerator<double, TKey> source, int partitionIndex, int sign, CancellationToken cancellationToken)
				: base(partitionIndex, cancellationToken)
			{
				this._source = source;
				this._sign = sign;
			}

			protected override bool MoveNextCore(ref double currentElement)
			{
				QueryOperatorEnumerator<double, TKey> source = this._source;
				TKey tkey = default(TKey);
				if (source.MoveNext(ref currentElement, ref tkey))
				{
					int num = 0;
					if (this._sign == -1)
					{
						double num2 = 0.0;
						while (source.MoveNext(ref num2, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num2 < currentElement || double.IsNaN(num2))
							{
								currentElement = num2;
							}
						}
					}
					else
					{
						double num3 = 0.0;
						while (source.MoveNext(ref num3, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							if (num3 > currentElement || double.IsNaN(currentElement))
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

			private QueryOperatorEnumerator<double, TKey> _source;

			private int _sign;
		}
	}
}
