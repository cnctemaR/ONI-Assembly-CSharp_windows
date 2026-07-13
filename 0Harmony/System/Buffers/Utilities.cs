using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	internal static class Utilities
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int SelectBucketIndex(int bufferSize)
		{
			return BitOperations.Log2((uint)((bufferSize - 1) | 15)) - 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetMaxSizeForBucket(int binIndex)
		{
			return 16 << binIndex;
		}

		internal static Utilities.MemoryPressure GetMemoryPressure()
		{
			return Utilities.MemoryPressure.Low;
		}

		internal enum MemoryPressure
		{
			Low,
			Medium,
			High
		}
	}
}
