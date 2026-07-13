using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Profiling
{
	[NativeHeader("NativeKernel/Allocator/MemoryManager.h")]
	[NativeHeader("NativeKernel/Utilities/MemoryUtilities.h")]
	[NativeHeader("Runtime/ScriptingBackend/ScriptingApi.h")]
	[NativeHeader("Runtime/Profiler/ScriptBindings/Profiler.bindings.h")]
	[NativeHeader("Runtime/Profiler/Profiler.h")]
	[NativeHeader("Runtime/Profiler/MemoryProfiler.h")]
	[MovedFrom("UnityEngine")]
	[UsedByNativeCode]
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
		public unsafe static string logFile
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Profiler.get_logFile_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Profiler.set_logFile_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
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

		[FreeFunction("ProfilerBindings::profiler_set_area_enabled")]
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

		[FreeFunction("ProfilerBindings::profiler_is_area_enabled")]
		[NativeConditional("ENABLE_PROFILER")]
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

		[NativeMethod(Name = "ProfilerBindings::SetScreenshotCaptureFrameInterval", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetScreenshotCaptureFrameInterval(int frames);

		[NativeConditional("ENABLE_PROFILER && UNITY_EDITOR")]
		[NativeHeader("Modules/ProfilerEditor/Public/ProfilerSession.h")]
		[NativeMethod(Name = "LoadFromFile")]
		[StaticAccessor("profiling::GetProfilerSessionPtr()", StaticAccessorType.Arrow)]
		private unsafe static void AddFramesFromFile_Internal(string file, bool keepExistingFrames)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(file, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = file.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Profiler.AddFramesFromFile_Internal_Injected(ref managedSpanWrapper, keepExistingFrames);
			}
			finally
			{
				char* ptr = null;
			}
		}

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

		[NativeMethod(Name = "ProfilerBindings::BeginThreadProfiling", IsFreeFunction = true, IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		private unsafe static void BeginThreadProfilingInternal(string threadGroupName, string threadName)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(threadGroupName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = threadGroupName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(threadName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = threadName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				Profiler.BeginThreadProfilingInternal_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeConditional("ENABLE_PROFILER")]
		public static void EndThreadProfiling()
		{
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BeginSample(string name)
		{
			Profiler.ValidateArguments(name);
			Profiler.BeginSampleImpl(name, null);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BeginSample(string name, Object targetObject)
		{
			Profiler.ValidateArguments(name);
			Profiler.BeginSampleImpl(name, targetObject);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateArguments(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Argument should be a valid string.", "name");
			}
		}

		[NativeMethod(Name = "ProfilerBindings::BeginSample", IsFreeFunction = true, IsThreadSafe = true)]
		private unsafe static void BeginSampleImpl(string name, Object targetObject)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Profiler.BeginSampleImpl_Injected(ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<Object>(targetObject));
			}
			finally
			{
				char* ptr = null;
			}
		}

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

		[NativeMethod(Name = "ProfilerBindings::GetRuntimeMemorySizeLong", IsFreeFunction = true)]
		public static long GetRuntimeMemorySizeLong([NotNull] Object o)
		{
			if (o == null)
			{
				ThrowHelper.ThrowArgumentNullException(o, "o");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(o);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(o, "o");
			}
			return Profiler.GetRuntimeMemorySizeLong_Injected(intPtr);
		}

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

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTempAllocatorRequestedSize(uint size);

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTempAllocatorSize();

		[Obsolete("GetTotalAllocatedMemory has been deprecated since it is limited to 4GB. Please use GetTotalAllocatedMemoryLong() instead.")]
		public static uint GetTotalAllocatedMemory()
		{
			return (uint)Profiler.GetTotalAllocatedMemoryLong();
		}

		[NativeMethod(Name = "GetTotalAllocatedMemory")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalAllocatedMemoryLong();

		[Obsolete("GetTotalUnusedReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalUnusedReservedMemoryLong() instead.")]
		public static uint GetTotalUnusedReservedMemory()
		{
			return (uint)Profiler.GetTotalUnusedReservedMemoryLong();
		}

		[NativeMethod(Name = "GetTotalUnusedReservedMemory")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalUnusedReservedMemoryLong();

		[Obsolete("GetTotalReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalReservedMemoryLong() instead.")]
		public static uint GetTotalReservedMemory()
		{
			return (uint)Profiler.GetTotalReservedMemoryLong();
		}

		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeMethod(Name = "GetTotalReservedMemory")]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalReservedMemoryLong();

		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		public static long GetTotalFragmentationInfo(NativeArray<int> stats)
		{
			return Profiler.InternalGetTotalFragmentationInfo((IntPtr)stats.GetUnsafePtr<int>(), stats.Length);
		}

		[NativeMethod(Name = "GetTotalFragmentationInfo")]
		[StaticAccessor("GetMemoryManager()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_MEMORY_MANAGER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long InternalGetTotalFragmentationInfo(IntPtr pStats, int count);

		[NativeMethod(Name = "GetRegisteredGFXDriverMemory", IsThreadSafe = true)]
		[StaticAccessor("MemoryProfiler", StaticAccessorType.DoubleColon)]
		[NativeConditional("ENABLE_PROFILER")]
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
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, NoAllocHelpers.ExtractArrayFromList<T>(data), data.Count, UnsafeUtility.SizeOf(typeFromHandle), true);
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
			Profiler.Internal_EmitGlobalMetaData_Array((void*)(&id), 16, tag, NoAllocHelpers.ExtractArrayFromList<T>(data), data.Count, UnsafeUtility.SizeOf(typeFromHandle), false);
		}

		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitSessionMetaData<T>(Guid id, int tag, NativeArray<T> data) where T : struct
		{
			Profiler.Internal_EmitGlobalMetaData_Native((void*)(&id), 16, tag, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), data.Length, UnsafeUtility.SizeOf<T>(), false);
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "ProfilerBindings::Internal_EmitGlobalMetaData_Array", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_EmitGlobalMetaData_Array(void* id, int idLen, int tag, Array data, int count, int elementSize, bool frameData);

		[NativeMethod(Name = "ProfilerBindings::Internal_EmitGlobalMetaData_Native", IsFreeFunction = true, IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_EmitGlobalMetaData_Native(void* id, int idLen, int tag, IntPtr data, int count, int elementSize, bool frameData);

		[Conditional("ENABLE_PROFILER")]
		public static void SetCategoryEnabled(ProfilerCategory category, bool enabled)
		{
			bool flag = category == ProfilerCategory.Any;
			if (flag)
			{
				throw new ArgumentException("Argument should be a valid category", "category");
			}
			Profiler.Internal_SetCategoryEnabled(category, enabled);
		}

		public static bool IsCategoryEnabled(ProfilerCategory category)
		{
			bool flag = category == ProfilerCategory.Any;
			if (flag)
			{
				throw new ArgumentException("Argument should be a valid category", "category");
			}
			return Profiler.Internal_IsCategoryEnabled(category);
		}

		[StaticAccessor("profiling::GetProfilerManagerPtr()", StaticAccessorType.Arrow)]
		[NativeMethod(Name = "GetCategoriesCount")]
		[NativeHeader("Runtime/Profiler/ProfilerManager.h")]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetCategoriesCount();

		[Conditional("ENABLE_PROFILER")]
		public static void GetAllCategories(ProfilerCategory[] categories)
		{
			int num = 0;
			while ((long)num < Math.Min((long)((ulong)Profiler.GetCategoriesCount()), (long)categories.Length))
			{
				categories[num] = new ProfilerCategory((ushort)num);
				num++;
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void GetAllCategories(NativeArray<ProfilerCategory> categories)
		{
			int num = 0;
			while ((long)num < Math.Min((long)((ulong)Profiler.GetCategoriesCount()), (long)categories.Length))
			{
				categories[num] = new ProfilerCategory((ushort)num);
				num++;
			}
		}

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod(Name = "profiler_set_category_enable", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetCategoryEnabled(ushort categoryId, bool enabled);

		[NativeMethod(Name = "profiler_is_category_enabled", IsFreeFunction = true, IsThreadSafe = true)]
		[NativeConditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsCategoryEnabled(ushort categoryId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_logFile_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_logFile_Injected(ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddFramesFromFile_Internal_Injected(ref ManagedSpanWrapper file, bool keepExistingFrames);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginThreadProfilingInternal_Injected(ref ManagedSpanWrapper threadGroupName, ref ManagedSpanWrapper threadName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSampleImpl_Injected(ref ManagedSpanWrapper name, IntPtr targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetRuntimeMemorySizeLong_Injected(IntPtr o);

		internal const uint invalidProfilerArea = 4294967295U;
	}
}
