using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal static class BitOperationUtils
	{
		internal static int CountBits(int mask)
		{
			return BitOperationUtils.CountBits((uint)mask);
		}

		internal static int CountBits(uint mask)
		{
			uint num = 0U;
			while (mask > 0U)
			{
				num += mask & 1U;
				mask >>= 1;
			}
			return (int)num;
		}

		private static bool IsValueWithinMaskedBitsRange(uint value, uint mask, int bitCount)
		{
			return BitOperationUtils.AnyBitMatch(mask, value) && BitOperationUtils.IsValueSmallerOrEqualThanIndex(value, BitOperationUtils.BitCountToIndex(bitCount));
		}

		internal static uint ModifyMaskByValuesArrayAndBitCount(uint mask, IEnumerable<int> values, int bitCount = 32)
		{
			BitOperationUtils.AssertBitCount(bitCount);
			uint num = 0U;
			foreach (int num2 in values)
			{
				uint num3 = (uint)num2;
				bool flag = BitOperationUtils.IsValueWithinMaskedBitsRange(num3, mask, bitCount);
				if (flag)
				{
					num += num3;
				}
			}
			return num;
		}

		internal static bool AreAllBitsSetForValues(uint mask, IEnumerable<int> values, int bitCount = 32)
		{
			BitOperationUtils.AssertBitCount(bitCount);
			foreach (int num in values)
			{
				uint num2 = (uint)num;
				bool flag = BitOperationUtils.AnyBitMatch(mask, num2);
				bool flag2 = !flag || BitOperationUtils.IsValueBiggerOrEqualThanIndex(num2, BitOperationUtils.BitCountToIndex(bitCount));
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint IndexToValue(int index)
		{
			BitOperationUtils.AssertIndex(index);
			return 1U << index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsValueSmallerThanIndex(uint value, int index)
		{
			BitOperationUtils.AssertIndex(index);
			return value < BitOperationUtils.IndexToValue(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsValueBiggerThanIndex(uint value, int index)
		{
			BitOperationUtils.AssertIndex(index);
			return value > BitOperationUtils.IndexToValue(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsValueSmallerOrEqualThanIndex(uint value, int index)
		{
			BitOperationUtils.AssertIndex(index);
			return value <= BitOperationUtils.IndexToValue(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsValueBiggerOrEqualThanIndex(uint value, int index)
		{
			BitOperationUtils.AssertIndex(index);
			return value >= BitOperationUtils.IndexToValue(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool AnyBitMatch(uint mask, uint value)
		{
			return (mask & value) > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int BitCountToIndex(int bitCount)
		{
			BitOperationUtils.AssertBitCount(bitCount);
			return bitCount - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AssertBitCount(int bitCount)
		{
			Debug.Assert(bitCount >= 1 && bitCount <= 32, "Bit count must be between 1 and 32.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AssertIndex(int index)
		{
			Debug.Assert(index >= 0 && index <= 31, "Index must be between 0 and 31.");
		}
	}
}
