using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Profiling
{
	public readonly struct ProfilerMarker<[IsUnmanaged] TP1> where TP1 : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(string name, string param1Name)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(ProfilerCategory category, string name, string param1Name)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(TP1 p1)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker<TP1>.AutoScope Auto(TP1 p1)
		{
			return default(ProfilerMarker<TP1>.AutoScope);
		}

		public readonly struct AutoScope : IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal AutoScope(ProfilerMarker<TP1> marker, TP1 p1)
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
			}
		}
	}
}
