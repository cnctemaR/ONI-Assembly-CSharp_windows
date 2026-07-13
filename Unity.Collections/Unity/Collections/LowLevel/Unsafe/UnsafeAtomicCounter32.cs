using System;
using System.Threading;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public struct UnsafeAtomicCounter32
	{
		public unsafe UnsafeAtomicCounter32(void* ptr)
		{
			this.Counter = (int*)ptr;
		}

		public unsafe void Reset(int value = 0)
		{
			*this.Counter = value;
		}

		public unsafe int Add(int value)
		{
			return Interlocked.Add(UnsafeUtility.AsRef<int>((void*)this.Counter), value) - value;
		}

		public int Sub(int value)
		{
			return this.Add(-value);
		}

		public unsafe int AddSat(int value, int max = 2147483647)
		{
			int num = *this.Counter;
			int num2;
			do
			{
				num2 = num;
				num = ((num >= max) ? max : math.min(max, num + value));
				num = Interlocked.CompareExchange(UnsafeUtility.AsRef<int>((void*)this.Counter), num, num2);
			}
			while (num2 != num && num2 != max);
			return num2;
		}

		public unsafe int SubSat(int value, int min = -2147483648)
		{
			int num = *this.Counter;
			int num2;
			do
			{
				num2 = num;
				num = ((num <= min) ? min : math.max(min, num - value));
				num = Interlocked.CompareExchange(UnsafeUtility.AsRef<int>((void*)this.Counter), num, num2);
			}
			while (num2 != num && num2 != min);
			return num2;
		}

		public unsafe int* Counter;
	}
}
