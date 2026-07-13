using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Profiling
{
	public readonly struct ProfilerMarker<[IsUnmanaged] TP1, [IsUnmanaged] TP2, [IsUnmanaged] TP3> where TP1 : struct, ValueType where TP2 : struct, ValueType where TP3 : struct, ValueType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(string name, string param1Name, string param2Name, string param3Name)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(ProfilerCategory category, string name, string param1Name, string param2Name, string param3Name)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(TP1 p1, TP2 p2, TP3 p3)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker<TP1, TP2, TP3>.AutoScope Auto(TP1 p1, TP2 p2, TP3 p3)
		{
			return default(ProfilerMarker<TP1, TP2, TP3>.AutoScope);
		}

		public readonly struct AutoScope : IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal AutoScope(ProfilerMarker<TP1, TP2, TP3> marker, TP1 p1, TP2 p2, TP3 p3)
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
			}
		}
	}
}
