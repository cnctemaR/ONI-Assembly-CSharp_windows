using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Profiling
{
	public readonly struct ProfilerCounter<[IsUnmanaged] T> where T : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounter(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Sample(T value)
		{
		}
	}
}
