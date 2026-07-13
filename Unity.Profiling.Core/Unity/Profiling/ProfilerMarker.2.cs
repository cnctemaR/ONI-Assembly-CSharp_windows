using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Profiling
{
	public readonly struct ProfilerMarker<[IsUnmanaged] TP1, [IsUnmanaged] TP2> where TP1 : struct, ValueType where TP2 : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(string name, string param1Name, string param2Name)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(ProfilerCategory category, string name, string param1Name, string param2Name)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(TP1 p1, TP2 p2)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker<TP1, TP2>.AutoScope Auto(TP1 p1, TP2 p2)
		{
			return default(ProfilerMarker<TP1, TP2>.AutoScope);
		}

		public readonly struct AutoScope : IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal AutoScope(ProfilerMarker<TP1, TP2> marker, TP1 p1, TP2 p2)
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
			}
		}
	}
}
