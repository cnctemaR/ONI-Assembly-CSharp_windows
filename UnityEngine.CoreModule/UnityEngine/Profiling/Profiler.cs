using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/ScriptingBackend/ScriptingApi.h")]
	[NativeHeader("Runtime/Profiler/Profiler.h")]
	[NativeHeader("Runtime/Allocator/MemoryManager.h")]
	[MovedFrom("UnityEngine")]
	[NativeHeader("Runtime/Utilities/MemoryUtilities.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Profiler/ScriptBindings/Profiler.bindings.h")]
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
			[NativeMethod(Name = "profiler_is_enabled", IsFreeFunction = true, IsThreadSafe = true)]
			[NativeConditional("ENABLE_PROFILER")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod(Name = "ProfilerBindings::SetProfilerEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableAllocationCallstacks
		{
			[NativeMethod(Name = "ProfilerBindings::IsAllocationCallstackCaptureEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod(Name = "ProfilerBindings::SetAllocationCallstackCaptureEnabled", IsFreeFunction = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("profiler_set_area_enabled")]
		[Conditional("ENABLE_PROFILER")]
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
			bool flag = string.IsNullOrEmpty(file);
			if (flag)
			{
				Debug.LogError("AddFramesFromFile: Invalid or empty path");
			}
			else
			{
				Profiler.AddFramesFromFile_Internal(file, true);
			}
		}

		[StaticAccessor("profiling::GetProfilerSessionPtr()", StaticAccessorType.Arrow)]
		[NativeMethod(Name = "LoadFromFile")]
		[NativeHeader("Modules/ProfilerEditor/Public/ProfilerSession.h")]
		[NativeConditional("ENABLE_PROFILER && UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddFramesFromFile_Internal(string file, bool keepExistingFrames);

		[Conditional("ENABLE_PROFILER")]
		public static void BeginThreadProfiling(string threadGroupName, string threadName)
		{
			bool flag = string.IsNullOrEmpty(threadGroupName);
			if (flag)
			{
				throw new ArgumentException("Argument should be a valid string", "threadGroupName");
			}
			bool flag2 = string.IsNullOrEmpty(threadName);
			if (flag2)
			{
				throw new ArgumentException("Argument should be a valid string", "threadName");
			}
			Profiler.BeginThreadProfilingInternal(threadGroupName, threadName);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::BeginThreadProfiling", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginThreadProfilingInternal(string threadGroupName, string threadName);

		[NativeConditional("ENABLE_PROFILER")]
		public static void EndThreadProfiling()
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl((MethodImplOptions)256)]
		public static void BeginSample(string name)
		{
			Profiler.ValidateArguments(name);
			Profiler.BeginSampleImpl(name, null);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl((MethodImplOptions)256)]
		public static void BeginSample(string name, Object targetObject)
		{
			Profiler.ValidateArguments(name);
			Profiler.BeginSampleImpl(name, targetObject);
		}

		[MethodImpl((MethodImplOptions)256)]
		private static void ValidateArguments(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Argument should be a valid string.", "name");
			}
		}

		[NativeMethod(Name = "ProfilerBindings::BeginSample", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSampleImpl(string name, Object targetObject);

		[NativeMethod(Name = "ProfilerBindings::EndSample", IsFreeFunction = true, IsThreadSafe = true)]
		[Conditional("ENABLE_PROFILER")]
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

		[NativeMethod(Name = "ProfilerBindings::GetRuntimeMemorySizeLong", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetRuntimeMemorySizeLong([NotNull("ArgumentNullException")] Object o);

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

		[NativeMethod(Name = "GetTotalAllocatedMemory")]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalAllocatedMemoryLong();

		[Obsolete("GetTotalUnusedReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalUnusedReservedMemoryLong() instead.")]
		public static uint GetTotalUnusedReservedMemory()
		{
			return (uint)Profiler.GetTotalUnusedReservedMemoryLong();
		}

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeMethod(Name = "GetTotalUnusedReservedMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalUnusedReservedMemoryLong();

		[Obsolete("GetTotalReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalReservedMemoryLong() instead.")]
		public static uint GetTotalReservedMemory()
		{
			return (uint)Profiler.GetTotalReservedMemoryLong();
		}

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[NativeMethod(Name = "GetTotalReservedMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalReservedMemoryLong();

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		public static long GetTotalFragmentationInfo(NativeArray<int> stats)
		{
			return Profiler.InternalGetTotalFragmentationInfo((IntPtr)stats.GetUnsafePtr<int>(), stats.Length);
		}

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[NativeMethod(Name = "GetTotalFragmentationInfo")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long InternalGetTotalFragmentationInfo(IntPtr pStats, int count);

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "GetRegisteredGFXDriverMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetAllocatedMemoryForGraphicsDriver();

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitFrameMetaData(Guid id, int tag, Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Type elementType = data.GetType().GetElementType();
			bool flag2 = !UnsafeUtility.IsBlittable(elementType);
			if (flag2)
			{
				throw new ArgumentException(string.Format("{0} type must be blittable", elementType));
			}
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, data, data.Length, UnsafeUtility.SizeOf(elementType), true);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitFrameMetaData<T>(Guid id, int tag, List<T> data) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Type typeFromHandle = typeof(T);
			bool flag2 = !UnsafeUtility.IsBlittable(typeof(T));
			if (flag2)
			{
				throw new ArgumentException(string.Format("{0} type must be blittable", typeFromHandle));
			}
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, NoAllocHelpers.ExtractArrayFromList(data), data.Count, UnsafeUtility.SizeOf(typeFromHandle), true);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitFrameMetaData<T>(Guid id, int tag, NativeArray<T> data) where T : struct
		{
			Profiler.Internal_EmitGlobalMetaData_Native((void*)(&id), 16, tag, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), data.Length, UnsafeUtility.SizeOf<T>(), true);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitSessionMetaData(Guid id, int tag, Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Type elementType = data.GetType().GetElementType();
			bool flag2 = !UnsafeUtility.IsBlittable(elementType);
			if (flag2)
			{
				throw new ArgumentException(string.Format("{0} type must be blittable", elementType));
			}
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, data, data.Length, UnsafeUtility.SizeOf(elementType), false);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitSessionMetaData<T>(Guid id, int tag, List<T> data) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Type typeFromHandle = typeof(T);
			bool flag2 = !UnsafeUtility.IsBlittable(typeof(T));
			if (flag2)
			{
				throw new ArgumentException(string.Format("{0} type must be blittable", typeFromHandle));
			}
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, NoAllocHelpers.ExtractArrayFromList(data), data.Count, UnsafeUtility.SizeOf(typeFromHandle), false);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitSessionMetaData<T>(Guid id, int tag, NativeArray<T> data) where T : struct
		{
			Profiler.Internal_EmitGlobalMetaData_Native((void*)(&id), 16, tag, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), data.Length, UnsafeUtility.SizeOf<T>(), false);
		}

		[NativeMethod(Name = "ProfilerBindings::Internal_EmitGlobalMetaData_Array", IsFreeFunction = true, IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_EmitGlobalMetaData_Array(void* id, int idLen, int tag, Array data, int count, int elementSize, bool frameData);

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::Internal_EmitGlobalMetaData_Native", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_EmitGlobalMetaData_Native(void* id, int idLen, int tag, IntPtr data, int count, int elementSize, bool frameData);

		internal const uint invalidProfilerArea = 4294967295U;
	}
}
