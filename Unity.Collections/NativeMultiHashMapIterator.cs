using System;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeMultiHashMapIterator<TKey> where TKey : struct
	{
		public int GetEntryIndex()
		{
			return this.EntryIndex;
		}

		internal TKey key;

		internal int NextEntryIndex;

		internal int EntryIndex;
	}
}
