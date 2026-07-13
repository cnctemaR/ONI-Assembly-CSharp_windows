using System;
using System.Threading;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public struct UnsafeAtomicCounter64
	{
		public unsafe UnsafeAtomicCounter64(void* ptr)
		{
			this.Counter = (long*)ptr;
		}

		public unsafe void Reset(long value = 0L)
		{
			*this.Counter = value;
		}

		public unsafe long Add(long value)
		{
			return Interlocked.Add(UnsafeUtility.AsRef<long>((void*)this.Counter), value) - value;
		}

		public long Sub(long value)
		{
			return this.Add(-value);
		}

		public unsafe long AddSat(long value, long max = 9223372036854775807L)
		{
			long num = *this.Counter;
			long num2;
			do
			{
				num2 = num;
				num = ((num >= max) ? max : math.min(max, num + value));
				num = Interlocked.CompareExchange(UnsafeUtility.AsRef<long>((void*)this.Counter), num, num2);
			}
			while (num2 != num && num2 != max);
			return num2;
		}

		public unsafe long SubSat(long value, long min = -9223372036854775808L)
		{
			long num = *this.Counter;
			long num2;
			do
			{
				num2 = num;
				num = ((num <= min) ? min : math.max(min, num - value));
				num = Interlocked.CompareExchange(UnsafeUtility.AsRef<long>((void*)this.Counter), num, num2);
			}
			while (num2 != num && num2 != min);
			return num2;
		}

		public unsafe long* Counter;
	}
}
