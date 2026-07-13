using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Profiling
{
	public readonly struct ProfilerCounterValue<[IsUnmanaged] T> where T : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounterValue(string name)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounterValue(string name, ProfilerMarkerDataUnit dataUnit)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounterValue(string name, ProfilerMarkerDataUnit dataUnit, ProfilerCounterOptions counterOptions)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounterValue(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerCounterValue(ProfilerCategory category, string name, ProfilerMarkerDataUnit dataUnit, ProfilerCounterOptions counterOptions)
		{
		}

		public T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return default(T);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public void Sample()
		{
		}
	}
}
