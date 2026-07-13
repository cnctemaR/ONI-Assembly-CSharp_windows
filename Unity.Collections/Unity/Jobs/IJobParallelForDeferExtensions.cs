using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	public static class IJobParallelForDeferExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParallelForDefer
		{
			IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.Initialize();
		}

		public unsafe static JobHandle Schedule<T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T jobData, NativeList<U> list, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer where U : struct, ValueType
		{
			void* ptr = null;
			return IJobParallelForDeferExtensions.ScheduleInternal<T>(ref jobData, innerloopBatchCount, NativeListUnsafeUtility.GetInternalListDataPtrUnchecked<U>(ref list), ptr, dependsOn);
		}

		public unsafe static JobHandle ScheduleByRef<T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T jobData, NativeList<U> list, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer where U : struct, ValueType
		{
			void* ptr = null;
			return IJobParallelForDeferExtensions.ScheduleInternal<T>(ref jobData, innerloopBatchCount, NativeListUnsafeUtility.GetInternalListDataPtrUnchecked<U>(ref list), ptr, dependsOn);
		}

		public unsafe static JobHandle Schedule<T>(this T jobData, int* forEachCount, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer
		{
			byte* ptr = (byte*)(forEachCount - sizeof(void*) / 4);
			return IJobParallelForDeferExtensions.ScheduleInternal<T>(ref jobData, innerloopBatchCount, (void*)ptr, null, dependsOn);
		}

		public unsafe static JobHandle ScheduleByRef<T>(this T jobData, int* forEachCount, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForDefer
		{
			byte* ptr = (byte*)(forEachCount - sizeof(void*) / 4);
			return IJobParallelForDeferExtensions.ScheduleInternal<T>(ref jobData, innerloopBatchCount, (void*)ptr, null, dependsOn);
		}

		private unsafe static JobHandle ScheduleInternal<T>(ref T jobData, int innerloopBatchCount, void* forEachListPtr, void* atomicSafetyHandlePtr, JobHandle dependsOn) where T : struct, IJobParallelForDefer
		{
			IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.Initialize();
			IntPtr intPtr = *IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.jobReflectionData.Data;
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), intPtr, dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForDeferArraySize(ref jobScheduleParameters, innerloopBatchCount, forEachListPtr, atomicSafetyHandlePtr);
		}

		internal struct JobParallelForDeferProducer<T> where T : struct, IJobParallelForDefer
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				if (*IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.jobReflectionData.Data == IntPtr.Zero)
				{
					*IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(T), new IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.ExecuteJobFunction(IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>.Execute), null, null);
				}
			}

			public static void Execute(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				int num;
				int num2;
				while (JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out num, out num2))
				{
					int num3 = num2;
					for (int i = num; i < num3; i++)
					{
						jobData.Execute(i);
					}
				}
			}

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<IJobParallelForDeferExtensions.JobParallelForDeferProducer<T>>(0U);

			public delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
		}
	}
}
