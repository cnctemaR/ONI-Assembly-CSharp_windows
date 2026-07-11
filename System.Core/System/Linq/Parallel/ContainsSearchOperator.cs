using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ContainsSearchOperator<TInput> : UnaryQueryOperator<TInput, bool>
	{
		internal ContainsSearchOperator(IEnumerable<TInput> child, TInput searchValue, IEqualityComparer<TInput> comparer)
			: base(child)
		{
			this._searchValue = searchValue;
			if (comparer == null)
			{
				this._comparer = EqualityComparer<TInput>.Default;
				return;
			}
			this._comparer = comparer;
		}

		internal bool Aggregate()
		{
			using (IEnumerator<bool> enumerator = this.GetEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true))
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal override QueryResults<bool> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TInput, bool>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<bool> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<bool, int> partitionedStream = new PartitionedStream<bool, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			Shared<bool> shared = new Shared<bool>(false);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new ContainsSearchOperator<TInput>.ContainsSearchOperatorEnumerator<TKey>(inputStream[i], this._searchValue, this._comparer, i, shared, settings.CancellationState.MergedCancellationToken);
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

		private readonly TInput _searchValue;

		private readonly IEqualityComparer<TInput> _comparer;

		private class ContainsSearchOperatorEnumerator<TKey> : QueryOperatorEnumerator<bool, int>
		{
			internal ContainsSearchOperatorEnumerator(QueryOperatorEnumerator<TInput, TKey> source, TInput searchValue, IEqualityComparer<TInput> comparer, int partitionIndex, Shared<bool> resultFoundFlag, CancellationToken cancellationToken)
			{
				this._source = source;
				this._searchValue = searchValue;
				this._comparer = comparer;
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
					currentElement = false;
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
						if (this._comparer.Equals(tinput, this._searchValue))
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
					currentElement = true;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, TKey> _source;

			private readonly TInput _searchValue;

			private readonly IEqualityComparer<TInput> _comparer;

			private readonly int _partitionIndex;

			private readonly Shared<bool> _resultFoundFlag;

			private CancellationToken _cancellationToken;
		}
	}
}
