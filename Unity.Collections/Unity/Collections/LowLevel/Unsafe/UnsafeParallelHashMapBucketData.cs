using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public struct UnsafeParallelHashMapBucketData
	{
		internal unsafe UnsafeParallelHashMapBucketData(byte* v, byte* k, byte* n, byte* b, int bcm)
		{
			this.values = v;
			this.keys = k;
			this.next = n;
			this.buckets = b;
			this.bucketCapacityMask = bcm;
		}

		public unsafe readonly byte* values;

		public unsafe readonly byte* keys;

		public unsafe readonly byte* next;

		public unsafe readonly byte* buckets;

		public readonly int bucketCapacityMask;
	}
}
