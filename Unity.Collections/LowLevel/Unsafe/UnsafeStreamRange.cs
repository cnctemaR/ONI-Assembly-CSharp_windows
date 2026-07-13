using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct UnsafeStreamRange
	{
		internal unsafe UnsafeStreamBlock* Block;

		internal int OffsetInFirstBlock;

		internal int ElementCount;

		internal int LastOffset;

		internal int NumberOfBlocks;
	}
}
