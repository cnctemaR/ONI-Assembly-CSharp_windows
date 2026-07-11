using System;
using System.Runtime.Interop;
using System.Security;

namespace System.Runtime
{
	internal static class Ticks
	{
		public static long Now
		{
			[SecuritySafeCritical]
			get
			{
				long num;
				UnsafeNativeMethods.GetSystemTimeAsFileTime(out num);
				return num;
			}
		}

		public static long FromMilliseconds(int milliseconds)
		{
			checked
			{
				return unchecked((long)milliseconds) * 10000L;
			}
		}

		public static int ToMilliseconds(long ticks)
		{
			return checked((int)(ticks / 10000L));
		}

		public static long FromTimeSpan(TimeSpan duration)
		{
			return duration.Ticks;
		}

		public static TimeSpan ToTimeSpan(long ticks)
		{
			return new TimeSpan(ticks);
		}

		public static long Add(long firstTicks, long secondTicks)
		{
			if (firstTicks == 9223372036854775807L || firstTicks == -9223372036854775808L)
			{
				return firstTicks;
			}
			if (secondTicks == 9223372036854775807L || secondTicks == -9223372036854775808L)
			{
				return secondTicks;
			}
			if (firstTicks >= 0L && 9223372036854775807L - firstTicks <= secondTicks)
			{
				return 9223372036854775806L;
			}
			if (firstTicks <= 0L && -9223372036854775808L - firstTicks >= secondTicks)
			{
				return -9223372036854775807L;
			}
			return checked(firstTicks + secondTicks);
		}
	}
}
