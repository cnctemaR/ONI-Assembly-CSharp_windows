using System;
using System.Threading;
using Unity.Mathematics;

namespace Unity.Collections
{
	internal class ConcurrentMask
	{
		internal static long AtomicOr(ref long destination, long source)
		{
			long num = Interlocked.Read(ref destination);
			long num2;
			long num3;
			do
			{
				num2 = num | source;
				num3 = num;
				num = Interlocked.CompareExchange(ref destination, num2, num3);
			}
			while (num != num3);
			return num2;
		}

		internal static long AtomicAnd(ref long destination, long source)
		{
			long num = Interlocked.Read(ref destination);
			long num2;
			long num3;
			do
			{
				num2 = num & source;
				num3 = num;
				num = Interlocked.CompareExchange(ref destination, num2, num3);
			}
			while (num != num3);
			return num2;
		}

		internal static void longestConsecutiveOnes(long value, out int offset, out int count)
		{
			count = 0;
			long num = value;
			while (num != 0L)
			{
				value = num;
				num = value & (long)((ulong)value >> 1);
				count++;
			}
			offset = math.tzcnt(value);
		}

		internal static bool foundAtLeastThisManyConsecutiveOnes(long value, int minimum, out int offset, out int count)
		{
			if (minimum == 1)
			{
				offset = math.tzcnt(value);
				count = 1;
				return offset != 64;
			}
			ConcurrentMask.longestConsecutiveOnes(value, out offset, out count);
			return count >= minimum;
		}

		internal static bool foundAtLeastThisManyConsecutiveZeroes(long value, int minimum, out int offset, out int count)
		{
			return ConcurrentMask.foundAtLeastThisManyConsecutiveOnes(~value, minimum, out offset, out count);
		}

		internal static bool Succeeded(int error)
		{
			return error >= 0;
		}

		internal static long MakeMask(int offset, int bits)
		{
			return (long)((long)(ulong.MaxValue >> 64 - bits) << offset);
		}

		internal static int TryAllocate(ref long l, int offset, int bits)
		{
			long num = ConcurrentMask.MakeMask(offset, bits);
			long num2 = Interlocked.Read(ref l);
			while ((num2 & num) == 0L)
			{
				long num3 = num2 | num;
				long num4 = num2;
				num2 = Interlocked.CompareExchange(ref l, num3, num4);
				if (num2 == num4)
				{
					return math.countbits(num2);
				}
			}
			return -2;
		}

		internal static int TryFree(ref long l, int offset, int bits)
		{
			long num = ConcurrentMask.MakeMask(offset, bits);
			long num2 = Interlocked.Read(ref l);
			while ((num2 & num) == num)
			{
				long num3 = num2 & ~num;
				long num4 = num2;
				num2 = Interlocked.CompareExchange(ref l, num3, num4);
				if (num2 == num4)
				{
					return math.countbits(num3);
				}
			}
			return -1;
		}

		internal static int TryAllocate(ref long l, out int offset, int bits)
		{
			long num = Interlocked.Read(ref l);
			int num2;
			while (ConcurrentMask.foundAtLeastThisManyConsecutiveZeroes(num, bits, out offset, out num2))
			{
				long num3 = ConcurrentMask.MakeMask(offset, bits);
				long num4 = num | num3;
				long num5 = num;
				num = Interlocked.CompareExchange(ref l, num4, num5);
				if (num == num5)
				{
					return math.countbits(num);
				}
			}
			return -2;
		}

		internal static int TryAllocate<T>(ref T t, int offset, int bits) where T : IIndexable<long>
		{
			int num = offset >> 6;
			int num2 = offset & 63;
			if (num2 + bits > 64)
			{
				return -3;
			}
			return ConcurrentMask.TryAllocate(t.ElementAt(num), num2, bits);
		}

		internal static int TryFree<T>(ref T t, int offset, int bits) where T : IIndexable<long>
		{
			int num = offset >> 6;
			int num2 = offset & 63;
			return ConcurrentMask.TryFree(t.ElementAt(num), num2, bits);
		}

		internal unsafe static int TryAllocate<T>(ref T t, out int offset, int begin, int end, int bits) where T : IIndexable<long>
		{
			for (int i = begin; i < end; i++)
			{
				if (*t.ElementAt(i) != -1L)
				{
					IL_004D:
					while (i < end)
					{
						int num2;
						int num = ConcurrentMask.TryAllocate(t.ElementAt(i), out num2, bits);
						if (ConcurrentMask.Succeeded(num))
						{
							offset = i * 64 + num2;
							return num;
						}
						i++;
					}
					offset = -1;
					return -2;
				}
			}
			goto IL_004D;
		}

		internal static int TryAllocate<T>(ref T t, out int offset, int begin, int bits) where T : IIndexable<long>
		{
			int num = ConcurrentMask.TryAllocate<T>(ref t, out offset, begin, t.Length, bits);
			if (ConcurrentMask.Succeeded(num))
			{
				return num;
			}
			return ConcurrentMask.TryAllocate<T>(ref t, out offset, 0, begin, bits);
		}

		internal static int TryAllocate<T>(ref T t, out int offset, int bits) where T : IIndexable<long>
		{
			return ConcurrentMask.TryAllocate<T>(ref t, out offset, 0, t.Length, bits);
		}

		internal const int ErrorFailedToFree = -1;

		internal const int ErrorFailedToAllocate = -2;

		internal const int ErrorAllocationCrossesWordBoundary = -3;

		internal const int EmptyBeforeAllocation = 0;

		internal const int EmptyAfterFree = 0;
	}
}
