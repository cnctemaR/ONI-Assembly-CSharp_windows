using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/Utilities/MemoryUtilities.h")]
	[NativeHeader("Runtime/Profiler/Profiler.h")]
	[NativeHeader("Runtime/Profiler/ScriptBindings/Profiler.bindings.h")]
	[NativeHeader("Runtime/Allocator/MemoryManager.h")]
	[UsedByNativeCode]
	[MovedFrom("UnityEngine")]
	[NativeHeader("Runtime/ScriptingBackend/ScriptingApi.h")]
	public sealed class Profiler
	{
		private Profiler()
		{
		}

		public static extern bool supported
		{
			[NativeMethod(Name = "profiler_is_available", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("ProfilerBindings", StaticAccessorType.DoubleColon)]
		public static extern string logFile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableBinaryLog
		{
			[NativeMethod(Name = "ProfilerBindings::IsBinaryLogEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod(Name = "ProfilerBindings::SetBinaryLogEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int maxUsedMemory
		{
			[NativeMethod(Name = "ProfilerBindings::GetMaxUsedMemory", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod(Name = "ProfilerBindings::SetMaxUsedMemory", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enabled
		{
			[NativeMethod(Name = "profiler_is_enabled", IsFreeFunction = true)]
			[NativeConditional("ENABLE_PROFILER")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod(Name = "ProfilerBindings::SetProfilerEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Conditional("ENABLE_PROFILER")]
		[FreeFunction("profiler_set_area_enabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAreaEnabled(ProfilerArea area, bool enabled);

		public static int areaCount
		{
			get
			{
				return Enum.GetNames(typeof(ProfilerArea)).Length;
			}
		}

		[NativeConditional("ENABLE_PROFILER")]
		[FreeFunction("profiler_is_area_enabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetAreaEnabled(ProfilerArea area);

		[Conditional("UNITY_EDITOR")]
		public static void AddFramesFromFile(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				Debug.LogError("AddFramesFromFile: Invalid or empty path");
			}
			else
			{
				Profiler.AddFramesFromFile_Internal(file, true);
			}
		}

		[NativeMethod(Name = "LoadFromFile")]
		[StaticAccessor("profiling::GetProfilerSessionPtr()", StaticAccessorType.Arrow)]
		[NativeConditional("ENABLE_PROFILER && UNITY_EDITOR")]
		[NativeHeader("Modules/ProfilerEditor/Public/ProfilerSession.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddFramesFromFile_Internal(string file, bool keepExistingFrames);

		[Conditional("ENABLE_PROFILER")]
		public static void BeginThreadProfiling(string threadGroupName, string threadName)
		{
			if (string.IsNullOrEmpty(threadGroupName))
			{
				throw new ArgumentException("Argument should be a valid string", "threadGroupName");
			}
			if (string.IsNullOrEmpty(threadName))
			{
				throw new ArgumentException("Argument should be a valid string", "threadName");
			}
			Profiler.BeginThreadProfilingInternal(threadGroupName, threadName);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::BeginThreadProfiling", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginThreadProfilingInternal(string threadGroupName, string threadName);

		[NativeMethod(Name = "ProfilerBindings::EndThreadProfiling", IsFreeFunction = true, IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndThreadProfiling();

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name)
		{
			Profiler.BeginSampleImpl(name, null);
		}

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name, Object targetObject)
		{
			Profiler.BeginSampleImpl(name, targetObject);
		}

		[NativeMethod(Name = "ProfilerBindings::BeginSample", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSampleImpl(string name, Object targetObject);

		[Conditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::EndSample", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSample();

		[Obsolete("maxNumberOfSamplesPerFrame has been depricated. Use maxUsedMemory instead")]
		public static int maxNumberOfSamplesPerFrame
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("usedHeapSize has been deprecated since it is limited to 4GB. Please use usedHeapSizeLong instead.")]
		public static uint usedHeapSize
		{
			get
			{
				return (uint)Profiler.usedHeapSizeLong;
			}
		}

		public static extern long usedHeapSizeLong
		{
			[NativeMethod(Name = "GetUsedHeapSize", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("GetRuntimeMemorySize has been deprecated since it is limited to 2GB. Please use GetRuntimeMemorySizeLong() instead.")]
		public static int GetRuntimeMemorySize(Object o)
		{
			return (int)Profiler.GetRuntimeMemorySizeLong(o);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::GetRuntimeMemorySizeLong", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetRuntimeMemorySizeLong(Object o);

		[Obsolete("GetMonoHeapSize has been deprecated since it is limited to 4GB. Please use GetMonoHeapSizeLong() instead.")]
		public static uint GetMonoHeapSize()
		{
			return (uint)Profiler.GetMonoHeapSizeLong();
		}

		[NativeMethod(Name = "scripting_gc_get_heap_size", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetMonoHeapSizeLong();

		[Obsolete("GetMonoUsedSize has been deprecated since it is limited to 4GB. Please use GetMonoUsedSizeLong() instead.")]
		public static uint GetMonoUsedSize()
		{
			return (uint)Profiler.GetMonoUsedSizeLong();
		}

		[NativeMethod(Name = "scripting_gc_get_used_size", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetMonoUsedSizeLong();

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTempAllocatorRequestedSize(uint size);

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTempAllocatorSize();

		[Obsolete("GetTotalAllocatedMemory has been deprecated since it is limited to 4GB. Please use GetTotalAllocatedMemoryLong() instead.")]
		public static uint GetTotalAllocatedMemory()
		{
			return (uint)Profiler.GetTotalAllocatedMemoryLong();
		}

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeMethod(Name = "GetTotalAllocatedMemory")]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalAllocatedMemoryLong();

		[Obsolete("GetTotalUnusedReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalUnusedReservedMemoryLong() instead.")]
		public static uint GetTotalUnusedReservedMemory()
		{
			return (uint)Profiler.GetTotalUnusedReservedMemoryLong();
		}

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[NativeMethod(Name = "GetTotalUnusedReservedMemory")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalUnusedReservedMemoryLong();

		[Obsolete("GetTotalReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalReservedMemoryLong() instead.")]
		public static uint GetTotalReservedMemory()
		{
			return (uint)Profiler.GetTotalReservedMemoryLong();
		}

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeMethod(Name = "GetTotalReservedMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalReservedMemoryLong();

		[NativeMethod(Name = "GetRegisteredGFXDriverMemory")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetAllocatedMemoryForGraphicsDriver();

		internal const uint invalidProfilerArea = 4294967295U;
	}
}
