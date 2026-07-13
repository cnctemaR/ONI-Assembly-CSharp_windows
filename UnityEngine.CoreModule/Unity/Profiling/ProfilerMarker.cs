using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Profiling
{
	[IgnoredByDeepProfiler]
	[UsedByNativeCode]
	public struct ProfilerMarker
	{
		public IntPtr Handle
		{
			get
			{
				return this.m_Ptr;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(string name)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, 1, MarkerFlags.Default, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ProfilerMarker(char* name, int nameLen)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, 1, MarkerFlags.Default, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(ProfilerCategory category, string name)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, MarkerFlags.Default, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ProfilerMarker(ProfilerCategory category, char* name, int nameLen)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, category, MarkerFlags.Default, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker(ProfilerCategory category, string name, MarkerFlags flags)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, flags, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ProfilerMarker(ProfilerCategory category, char* name, int nameLen, MarkerFlags flags)
		{
			this.m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, category, flags, 0);
		}

		[Pure]
		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin()
		{
			ProfilerUnsafeUtility.BeginSample(this.m_Ptr);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(Object contextUnityObject)
		{
			ProfilerUnsafeUtility.Internal_BeginWithObject(this.m_Ptr, contextUnityObject);
		}

		[Conditional("ENABLE_PROFILER")]
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End()
		{
			ProfilerUnsafeUtility.EndSample(this.m_Ptr);
		}

		[Conditional("ENABLE_PROFILER")]
		internal void GetName(ref string name)
		{
			name = ProfilerUnsafeUtility.Internal_GetName(this.m_Ptr);
		}

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProfilerMarker.AutoScope Auto()
		{
			return new ProfilerMarker.AutoScope(this.m_Ptr);
		}

		[NativeDisableUnsafePtrRestriction]
		[NonSerialized]
		internal readonly IntPtr m_Ptr;

		[UsedByNativeCode]
		[IgnoredByDeepProfiler]
		public struct AutoScope : IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal AutoScope(IntPtr markerPtr)
			{
				this.m_Ptr = markerPtr;
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					ProfilerUnsafeUtility.BeginSample(markerPtr);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					ProfilerUnsafeUtility.EndSample(this.m_Ptr);
				}
			}

			[NativeDisableUnsafePtrRestriction]
			internal readonly IntPtr m_Ptr;
		}
	}
}
