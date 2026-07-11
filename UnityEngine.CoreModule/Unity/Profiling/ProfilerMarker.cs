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
		[MethodImpl((MethodImplOptions)256)]
		public ProfilerMarker(string name)
		{
			this.m_Ptr = ProfilerMarker.Internal_Create(name, 0);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl((MethodImplOptions)256)]
		public void Begin()
		{
			ProfilerMarker.Internal_Begin(this.m_Ptr);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl((MethodImplOptions)256)]
		public void Begin(global::UnityEngine.Object contextUnityObject)
		{
			ProfilerMarker.Internal_BeginWithObject(this.m_Ptr, contextUnityObject);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl((MethodImplOptions)256)]
		public void End()
		{
			ProfilerMarker.Internal_End(this.m_Ptr);
		}

		[Conditional("ENABLE_PROFILER")]
		internal void GetName(ref string name)
		{
			name = ProfilerMarker.Internal_GetName(this.m_Ptr);
		}

		[MethodImpl((MethodImplOptions)256)]
		public ProfilerMarker.AutoScope Auto()
		{
			return new ProfilerMarker.AutoScope(this.m_Ptr);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create(string name, ushort flags);

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Begin(IntPtr markerPtr);

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_BeginWithObject(IntPtr markerPtr, global::UnityEngine.Object contextUnityObject);

		[ThreadSafe]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_End(IntPtr markerPtr);

		[NativeConditional("ENABLE_PROFILER")]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void Internal_Emit(IntPtr markerPtr, ushort eventType, int metadataCount, void* metadata);

		[NativeConditional("ENABLE_PROFILER")]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetName(IntPtr markerPtr);

		[NativeDisableUnsafePtrRestriction]
		internal readonly IntPtr m_Ptr;

		[UsedByNativeCode]
		public struct AutoScope : IDisposable
		{
			[MethodImpl((MethodImplOptions)256)]
			internal AutoScope(IntPtr markerPtr)
			{
				this.m_Ptr = markerPtr;
				ProfilerMarker.Internal_Begin(markerPtr);
			}

			[MethodImpl((MethodImplOptions)256)]
			public void Dispose()
			{
				ProfilerMarker.Internal_End(this.m_Ptr);
			}

			[NativeDisableUnsafePtrRestriction]
			internal readonly IntPtr m_Ptr;
		}
	}
}
