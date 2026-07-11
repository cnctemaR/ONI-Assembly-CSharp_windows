using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class PartitionedStream<TElement, TKey>
	{
		internal PartitionedStream(int partitionCount, IComparer<TKey> keyComparer, OrdinalIndexState indexState)
		{
			this._partitions = new QueryOperatorEnumerator<TElement, TKey>[partitionCount];
			this._keyComparer = keyComparer;
			this._indexState = indexState;
		}

		internal QueryOperatorEnumerator<TElement, TKey> this[int index]
		{
			get
			{
				return this._partitions[index];
			}
			set
			{
				this._partitions[index] = value;
			}
		}

		public int PartitionCount
		{
			get
			{
				return this._partitions.Length;
			}
		}

		internal IComparer<TKey> KeyComparer
		{
			get
			{
				return this._keyComparer;
			}
		}

		internal OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return this._indexState;
			}
		}

		protected QueryOperatorEnumerator<TElement, TKey>[] _partitions;

		private readonly IComparer<TKey> _keyComparer;

		private readonly OrdinalIndexState _indexState;
	}
}
