using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeParallelMultiHashMapIterator<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey> where TKey : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetEntryIndex()
		{
			return this.EntryIndex;
		}

		internal TKey key;

		internal int NextEntryIndex;

		internal int EntryIndex;
	}
}
