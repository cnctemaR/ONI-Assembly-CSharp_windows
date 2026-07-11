using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class AnyAllSearchOperator<TInput> : UnaryQueryOperator<TInput, bool>
	{
		internal AnyAllSearchOperator(IEnumerable<TInput> child, bool qualification, Func<TInput, bool> predicate)
			: base(child)
		{
			this._qualification = qualification;
			this._predicate = predicate;
		}

		internal bool Aggregate()
		{
			using (IEnumerator<bool> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this._qualification)
					{
						return this._qualification;
					}
				}
			}
			return !this._qualification;
		}

		internal override QueryResults<bool> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TInput, bool>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<bool> recipient, bool preferStriping, QuerySettings settings)
		{
			Shared<bool> shared = new Shared<bool>(false);
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<bool, int> partitionedStream = new PartitionedStream<bool, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new AnyAllSearchOperator<TInput>.AnyAllSearchOperatorEnumerator<TKey>(inputStream[i], this._qualification, this._predicate, i, shared, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<int>(partitionedStream);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<bool> AsSequentialQuery(CancellationToken token)
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

		private readonly Func<TInput, bool> _predicate;

		private readonly bool _qualification;

		private class AnyAllSearchOperatorEnumerator<TKey> : QueryOperatorEnumerator<bool, int>
		{
			internal AnyAllSearchOperatorEnumerator(QueryOperatorEnumerator<TInput, TKey> source, bool qualification, Func<TInput, bool> predicate, int partitionIndex, Shared<bool> resultFoundFlag, CancellationToken cancellationToken)
			{
				this._source = source;
				this._qualification = qualification;
				this._predicate = predicate;
				this._partitionIndex = partitionIndex;
				this._resultFoundFlag = resultFoundFlag;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref bool currentElement, ref int currentKey)
			{
				if (this._resultFoundFlag.Value)
				{
					return false;
				}
				TInput tinput = default(TInput);
				TKey tkey = default(TKey);
				if (this._source.MoveNext(ref tinput, ref tkey))
				{
					currentElement = !this._qualification;
					currentKey = this._partitionIndex;
					int num = 0;
					for (;;)
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (this._resultFoundFlag.Value)
						{
							break;
						}
						if (this._predicate(tinput) == this._qualification)
						{
							goto Block_5;
						}
						if (!this._source.MoveNext(ref tinput, ref tkey))
						{
							return true;
						}
					}
					return false;
					Block_5:
					this._resultFoundFlag.Value = true;
					currentElement = this._qualification;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, TKey> _source;

			private readonly Func<TInput, bool> _predicate;

			private readonly bool _qualification;

			private readonly int _partitionIndex;

			private readonly Shared<bool> _resultFoundFlag;

			private readonly CancellationToken _cancellationToken;
		}
	}
}
