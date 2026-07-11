using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class SingleQueryOperator<TSource> : UnaryQueryOperator<TSource, TSource>
	{
		internal SingleQueryOperator(IEnumerable<TSource> child, Func<TSource, bool> predicate)
			: base(child)
		{
			this._predicate = predicate;
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults(base.Child.Open(settings, false), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TSource, int> partitionedStream = new PartitionedStream<TSource, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Shuffled);
			Shared<int> shared = new Shared<int>(0);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new SingleQueryOperator<TSource>.SingleQueryOperatorEnumerator<TKey>(inputStream[i], this._predicate, shared);
			}
			recipient.Receive<int>(partitionedStream);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
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

		private readonly Func<TSource, bool> _predicate;

		private class SingleQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TSource, int>
		{
			internal SingleQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, TKey> source, Func<TSource, bool> predicate, Shared<int> totalElementCount)
			{
				this._source = source;
				this._predicate = predicate;
				this._totalElementCount = totalElementCount;
			}

			internal override bool MoveNext(ref TSource currentElement, ref int currentKey)
			{
				if (!this._alreadySearched)
				{
					bool flag = false;
					TSource tsource = default(TSource);
					TKey tkey = default(TKey);
					while (this._source.MoveNext(ref tsource, ref tkey))
					{
						if (this._predicate == null || this._predicate(tsource))
						{
							Interlocked.Increment(ref this._totalElementCount.Value);
							currentElement = tsource;
							currentKey = 0;
							if (flag)
							{
								this._yieldExtra = true;
								break;
							}
							flag = true;
						}
						if (Volatile.Read(ref this._totalElementCount.Value) > 1)
						{
							break;
						}
					}
					this._alreadySearched = true;
					return flag;
				}
				if (this._yieldExtra)
				{
					this._yieldExtra = false;
					currentElement = default(TSource);
					currentKey = 0;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<TSource, TKey> _source;

			private Func<TSource, bool> _predicate;

			private bool _alreadySearched;

			private bool _yieldExtra;

			private Shared<int> _totalElementCount;
		}
	}
}
