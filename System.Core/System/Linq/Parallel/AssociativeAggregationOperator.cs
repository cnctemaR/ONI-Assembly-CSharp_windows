using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class AssociativeAggregationOperator<TInput, TIntermediate, TOutput> : UnaryQueryOperator<TInput, TIntermediate>
	{
		internal AssociativeAggregationOperator(IEnumerable<TInput> child, TIntermediate seed, Func<TIntermediate> seedFactory, bool seedIsSpecified, Func<TIntermediate, TInput, TIntermediate> intermediateReduce, Func<TIntermediate, TIntermediate, TIntermediate> finalReduce, Func<TIntermediate, TOutput> resultSelector, bool throwIfEmpty, QueryAggregationOptions options)
			: base(child)
		{
			this._seed = seed;
			this._seedFactory = seedFactory;
			this._seedIsSpecified = seedIsSpecified;
			this._intermediateReduce = intermediateReduce;
			this._finalReduce = finalReduce;
			this._resultSelector = resultSelector;
			this._throwIfEmpty = throwIfEmpty;
		}

		internal TOutput Aggregate()
		{
			TIntermediate tintermediate = default(TIntermediate);
			bool flag = false;
			using (IEnumerator<TIntermediate> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				while (enumerator.MoveNext())
				{
					if (flag)
					{
						try
						{
							tintermediate = this._finalReduce(tintermediate, enumerator.Current);
							continue;
						}
						catch (Exception ex)
						{
							throw new AggregateException(new Exception[] { ex });
						}
					}
					tintermediate = enumerator.Current;
					flag = true;
				}
				if (!flag)
				{
					if (this._throwIfEmpty)
					{
						throw new InvalidOperationException("Sequence contains no elements");
					}
					tintermediate = ((this._seedFactory == null) ? this._seed : this._seedFactory());
				}
			}
			TOutput toutput;
			try
			{
				toutput = this._resultSelector(tintermediate);
			}
			catch (Exception ex2)
			{
				throw new AggregateException(new Exception[] { ex2 });
			}
			return toutput;
		}

		internal override QueryResults<TIntermediate> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TInput, TIntermediate>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<TIntermediate> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TIntermediate, int> partitionedStream = new PartitionedStream<TIntermediate, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new AssociativeAggregationOperator<TInput, TIntermediate, TOutput>.AssociativeAggregationOperatorEnumerator<TKey>(inputStream[i], this, i, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<int>(partitionedStream);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TIntermediate> AsSequentialQuery(CancellationToken token)
		{
			throw new NotSupportedException();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly TIntermediate _seed;

		private readonly bool _seedIsSpecified;

		private readonly bool _throwIfEmpty;

		private Func<TIntermediate, TInput, TIntermediate> _intermediateReduce;

		private Func<TIntermediate, TIntermediate, TIntermediate> _finalReduce;

		private Func<TIntermediate, TOutput> _resultSelector;

		private Func<TIntermediate> _seedFactory;

		private class AssociativeAggregationOperatorEnumerator<TKey> : QueryOperatorEnumerator<TIntermediate, int>
		{
			internal AssociativeAggregationOperatorEnumerator(QueryOperatorEnumerator<TInput, TKey> source, AssociativeAggregationOperator<TInput, TIntermediate, TOutput> reduceOperator, int partitionIndex, CancellationToken cancellationToken)
			{
				this._source = source;
				this._reduceOperator = reduceOperator;
				this._partitionIndex = partitionIndex;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TIntermediate currentElement, ref int currentKey)
			{
				if (this._accumulated)
				{
					return false;
				}
				this._accumulated = true;
				bool flag = false;
				TIntermediate tintermediate = default(TIntermediate);
				if (this._reduceOperator._seedIsSpecified)
				{
					tintermediate = ((this._reduceOperator._seedFactory == null) ? this._reduceOperator._seed : this._reduceOperator._seedFactory());
				}
				else
				{
					TInput tinput = default(TInput);
					TKey tkey = default(TKey);
					if (!this._source.MoveNext(ref tinput, ref tkey))
					{
						return false;
					}
					flag = true;
					tintermediate = (TIntermediate)((object)tinput);
				}
				TInput tinput2 = default(TInput);
				TKey tkey2 = default(TKey);
				int num = 0;
				while (this._source.MoveNext(ref tinput2, ref tkey2))
				{
					if ((num++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					flag = true;
					tintermediate = this._reduceOperator._intermediateReduce(tintermediate, tinput2);
				}
				if (flag)
				{
					currentElement = tintermediate;
					currentKey = this._partitionIndex;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, TKey> _source;

			private readonly AssociativeAggregationOperator<TInput, TIntermediate, TOutput> _reduceOperator;

			private readonly int _partitionIndex;

			private readonly CancellationToken _cancellationToken;

			private bool _accumulated;
		}
	}
}
