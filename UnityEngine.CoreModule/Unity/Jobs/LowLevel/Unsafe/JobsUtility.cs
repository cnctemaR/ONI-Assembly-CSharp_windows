using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Jobs.LowLevel.Unsafe
{
	[NativeType(Header = "Runtime/Jobs/ScriptBindings/JobsBindings.h")]
	[NativeHeader("Runtime/Jobs/JobSystem.h")]
	public static class JobsUtility
	{
		public unsafe static void GetJobRange(ref JobRanges ranges, int jobIndex, out int beginIndex, out int endIndex)
		{
			int* ptr = (int*)(void*)ranges.StartEndIndex;
			beginIndex = ptr[jobIndex * 2];
			endIndex = ptr[jobIndex * 2 + 1];
		}

		[NativeMethod(IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetWorkStealingRange(ref JobRanges ranges, int jobIndex, out int beginIndex, out int endIndex);

		[FreeFunction("ScheduleManagedJob", ThrowsException = true, IsThreadSafe = true)]
		public static JobHandle Schedule(ref JobsUtility.JobScheduleParameters parameters)
		{
			JobHandle jobHandle;
			JobsUtility.Schedule_Injected(ref parameters, out jobHandle);
			return jobHandle;
		}

		[FreeFunction("ScheduleManagedJobParallelFor", ThrowsException = true, IsThreadSafe = true)]
		public static JobHandle ScheduleParallelFor(ref JobsUtility.JobScheduleParameters parameters, int arrayLength, int innerloopBatchCount)
		{
			JobHandle jobHandle;
			JobsUtility.ScheduleParallelFor_Injected(ref parameters, arrayLength, innerloopBatchCount, out jobHandle);
			return jobHandle;
		}

		[FreeFunction("ScheduleManagedJobParallelForDeferArraySize", ThrowsException = true, IsThreadSafe = true)]
		public unsafe static JobHandle ScheduleParallelForDeferArraySize(ref JobsUtility.JobScheduleParameters parameters, int innerloopBatchCount, void* listData, void* listDataAtomicSafetyHandle)
		{
			JobHandle jobHandle;
			JobsUtility.ScheduleParallelForDeferArraySize_Injected(ref parameters, innerloopBatchCount, listData, listDataAtomicSafetyHandle, out jobHandle);
			return jobHandle;
		}

		[FreeFunction("ScheduleManagedJobParallelForTransform", ThrowsException = true)]
		public static JobHandle ScheduleParallelForTransform(ref JobsUtility.JobScheduleParameters parameters, IntPtr transfromAccesssArray)
		{
			JobHandle jobHandle;
			JobsUtility.ScheduleParallelForTransform_Injected(ref parameters, transfromAccesssArray, out jobHandle);
			return jobHandle;
		}

		[FreeFunction("ScheduleManagedJobParallelForTransformReadOnly", ThrowsException = true)]
		public static JobHandle ScheduleParallelForTransformReadOnly(ref JobsUtility.JobScheduleParameters parameters, IntPtr transfromAccesssArray, int innerloopBatchCount)
		{
			JobHandle jobHandle;
			JobsUtility.ScheduleParallelForTransformReadOnly_Injected(ref parameters, transfromAccesssArray, innerloopBatchCount, out jobHandle);
			return jobHandle;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[NativeMethod(IsThreadSafe = true, IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void PatchBufferMinMaxRanges(IntPtr bufferRangePatchData, void* jobdata, int startIndex, int rangeSize);

		[FreeFunction(ThrowsException = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateJobReflectionData(Type wrapperJobType, Type userJobType, object managedJobFunction0, object managedJobFunction1, object managedJobFunction2);

		[Obsolete("JobType is obsolete. The parameter should be removed. (UnityUpgradable) -> !1")]
		public static IntPtr CreateJobReflectionData(Type type, JobType jobType, object managedJobFunction0, object managedJobFunction1 = null, object managedJobFunction2 = null)
		{
			return JobsUtility.CreateJobReflectionData(type, type, managedJobFunction0, managedJobFunction1, managedJobFunction2);
		}

		public static IntPtr CreateJobReflectionData(Type type, object managedJobFunction0, object managedJobFunction1 = null, object managedJobFunction2 = null)
		{
			return JobsUtility.CreateJobReflectionData(type, type, managedJobFunction0, managedJobFunction1, managedJobFunction2);
		}

		[Obsolete("JobType is obsolete. The parameter should be removed. (UnityUpgradable) -> !2")]
		public static IntPtr CreateJobReflectionData(Type wrapperJobType, Type userJobType, JobType jobType, object managedJobFunction0)
		{
			return JobsUtility.CreateJobReflectionData(wrapperJobType, userJobType, managedJobFunction0, null, null);
		}

		public static IntPtr CreateJobReflectionData(Type wrapperJobType, Type userJobType, object managedJobFunction0)
		{
			return JobsUtility.CreateJobReflectionData(wrapperJobType, userJobType, managedJobFunction0, null, null);
		}

		public static extern bool IsExecutingJob
		{
			[NativeMethod(Name = "GetIsExecutingScriptingJob", IsFreeFunction = true, IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool JobDebuggerEnabled
		{
			[FreeFunction]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool JobCompilerEnabled
		{
			[FreeFunction]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("JobSystem::GetJobQueueWorkerThreadCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJobQueueWorkerThreadCount();

		[FreeFunction("JobSystem::ForceSetJobQueueWorkerThreadCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetJobQueueMaximumActiveThreadCount(int count);

		public static extern int JobWorkerMaximumCount
		{
			[FreeFunction("JobSystem::GetJobQueueMaximumThreadCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("JobSystem::ResetJobQueueWorkerThreadCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetJobWorkerCount();

		public static int JobWorkerCount
		{
			get
			{
				return JobsUtility.GetJobQueueWorkerThreadCount();
			}
			set
			{
				bool flag = value < 0 || value > JobsUtility.JobWorkerMaximumCount;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("JobWorkerCount", string.Format("Invalid JobWorkerCount {0} must be in the range 0 -> {1}", value, JobsUtility.JobWorkerMaximumCount));
				}
				JobsUtility.SetJobQueueMaximumActiveThreadCount(value);
			}
		}

		public static extern int ThreadIndex
		{
			[FreeFunction("GetJobWorkerIndex", IsThreadSafe = true)]
			[BurstAuthorizedExternalMethod]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int ThreadIndexCount
		{
			[FreeFunction("GetJobWorkerIndexCount", IsThreadSafe = true)]
			[BurstAuthorizedExternalMethod]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("IsJobQueueBatchingEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetJobBatchingEnabled();

		internal static bool JobBatchingEnabled
		{
			get
			{
				return JobsUtility.GetJobBatchingEnabled();
			}
		}

		[FreeFunction("JobDebuggerGetSystemIdCellPtr")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSystemIdCellPtr();

		[FreeFunction("JobDebuggerClearSystemIds")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearSystemIds();

		[FreeFunction("JobDebuggerGetSystemIdMappings")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern int GetSystemIdMappings(JobHandle* handles, int* systemIds, int maxCount);

		[RequiredByNativeCode]
		private static void InvokePanicFunction()
		{
			JobsUtility.PanicFunction_ panicFunction = JobsUtility.PanicFunction;
			bool flag = panicFunction == null;
			if (!flag)
			{
				panicFunction();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Schedule_Injected(ref JobsUtility.JobScheduleParameters parameters, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleParallelFor_Injected(ref JobsUtility.JobScheduleParameters parameters, int arrayLength, int innerloopBatchCount, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ScheduleParallelForDeferArraySize_Injected(ref JobsUtility.JobScheduleParameters parameters, int innerloopBatchCount, void* listData, void* listDataAtomicSafetyHandle, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleParallelForTransform_Injected(ref JobsUtility.JobScheduleParameters parameters, IntPtr transfromAccesssArray, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleParallelForTransformReadOnly_Injected(ref JobsUtility.JobScheduleParameters parameters, IntPtr transfromAccesssArray, int innerloopBatchCount, out JobHandle ret);

		public const int MaxJobThreadCount = 128;

		public const int CacheLineSize = 64;

		internal static JobsUtility.PanicFunction_ PanicFunction;

		public struct JobScheduleParameters
		{
			public unsafe JobScheduleParameters(void* i_jobData, IntPtr i_reflectionData, JobHandle i_dependency, ScheduleMode i_scheduleMode)
			{
				this.Dependency = i_dependency;
				this.JobDataPtr = (IntPtr)i_jobData;
				this.ReflectionData = i_reflectionData;
				this.ScheduleMode = (int)i_scheduleMode;
			}

			public JobHandle Dependency;

			public int ScheduleMode;

			public IntPtr ReflectionData;

			public IntPtr JobDataPtr;
		}

		internal delegate void PanicFunction_();
	}
}
