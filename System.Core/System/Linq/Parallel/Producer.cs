using System;

namespace System.Linq.Parallel
{
	internal readonly struct Producer<TKey>
	{
		internal Producer(TKey maxKey, int producerIndex)
		{
			this.MaxKey = maxKey;
			this.ProducerIndex = producerIndex;
		}

		internal readonly TKey MaxKey;

		internal readonly int ProducerIndex;
	}
}
