using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	internal static class StopwatchExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ElapsedMillisecondsPrecise(this Stopwatch stopwatch)
		{
			return (double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0;
		}
	}
}
