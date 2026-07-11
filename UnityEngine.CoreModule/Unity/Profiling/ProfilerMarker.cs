using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Profiler/ScriptBindings/ProfilerMarker.bindings.h")]
	public struct ProfilerMarker
	{
		public ProfilerMarker(string name)
		{
			this.m_Ptr = ProfilerMarker.Internal_Create(name, MarkerFlags.Default);
		}

		[Conditional("ENABLE_PROFILER")]
		public void Begin()
		{
			ProfilerMarker.Internal_Begin(this.m_Ptr);
		}

		[Conditional("ENABLE_PROFILER")]
		public void Begin(global::UnityEngine.Object contextUnityObject)
		{
			ProfilerMarker.Internal_BeginWithObject(this.m_Ptr, contextUnityObject);
		}

		[Conditional("ENABLE_PROFILER")]
		public void End()
		{
			ProfilerMarker.Internal_End(this.m_Ptr);
		}

		public ProfilerMarker.AutoScope Auto()
		{
			return new ProfilerMarker.AutoScope(this.m_Ptr);
		}

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER", "NULL")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create(string name, MarkerFlags flags);

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Begin(IntPtr markerPtr);

		[NativeConditional("ENABLE_PROFILER")]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BeginWithObject(IntPtr markerPtr, global::UnityEngine.Object contextUnityObject);

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_End(IntPtr markerPtr);

		[NativeDisableUnsafePtrRestriction]
		internal IntPtr m_Ptr;

		[UsedByNativeCode]
		public struct AutoScope : IDisposable
		{
			internal AutoScope(IntPtr markerPtr)
			{
				this.m_Ptr = markerPtr;
				ProfilerMarker.Internal_Begin(markerPtr);
			}

			public void Dispose()
			{
				ProfilerMarker.Internal_End(this.m_Ptr);
			}

			[NativeDisableUnsafePtrRestriction]
			internal IntPtr m_Ptr;
		}
	}
}
