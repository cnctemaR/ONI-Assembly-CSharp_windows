using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class HashRepartitionStream<TInputOutput, THashKey, TOrderKey> : PartitionedStream<Pair<TInputOutput, THashKey>, TOrderKey>
	{
		internal HashRepartitionStream(int partitionsCount, IComparer<TOrderKey> orderKeyComparer, IEqualityComparer<THashKey> hashKeyComparer, IEqualityComparer<TInputOutput> elementComparer)
			: base(partitionsCount, orderKeyComparer, OrdinalIndexState.Shuffled)
		{
			this._keyComparer = hashKeyComparer;
			this._elementComparer = elementComparer;
			this._distributionMod = 503;
			checked
			{
				while (this._distributionMod < partitionsCount)
				{
					this._distributionMod *= 2;
				}
			}
		}

		internal int GetHashCode(TInputOutput element)
		{
			return (int.MaxValue & ((this._elementComparer == null) ? ((element == null) ? 0 : element.GetHashCode()) : this._elementComparer.GetHashCode(element))) % this._distributionMod;
		}

		internal int GetHashCode(THashKey key)
		{
			return (int.MaxValue & ((this._keyComparer == null) ? ((key == null) ? 0 : key.GetHashCode()) : this._keyComparer.GetHashCode(key))) % this._distributionMod;
		}

		private readonly IEqualityComparer<THashKey> _keyComparer;

		private readonly IEqualityComparer<TInputOutput> _elementComparer;

		private readonly int _distributionMod;

		private const int NULL_ELEMENT_HASH_CODE = 0;

		private const int HashCodeMask = 2147483647;
	}
}
