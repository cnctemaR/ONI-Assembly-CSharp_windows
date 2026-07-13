using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Jobs
{
	public static class IJobParallelForBatchExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobParallelForBatch
		{
			IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.Initialize();
		}

		private unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobParallelForBatch
		{
			IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.Initialize();
			return *IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.jobReflectionData.Data;
		}

		public static JobHandle Schedule<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Single);
			return JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, indicesPerJobCount);
		}

		public static JobHandle ScheduleByRef<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Single);
			return JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, indicesPerJobCount);
		}

		public static JobHandle ScheduleParallel<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, indicesPerJobCount);
		}

		public static JobHandle ScheduleParallelByRef<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, indicesPerJobCount);
		}

		public static JobHandle ScheduleBatch<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			return jobData.ScheduleParallel(arrayLength, indicesPerJobCount, dependsOn);
		}

		public static JobHandle ScheduleBatchByRef<T>(this T jobData, int arrayLength, int indicesPerJobCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForBatch
		{
			return (ref jobData).ScheduleParallelByRef<T>(arrayLength, indicesPerJobCount, dependsOn);
		}

		public static void Run<T>(this T jobData, int arrayLength, int indicesPerJobCount) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, arrayLength);
		}

		public static void RunByRef<T>(this T jobData, int arrayLength, int indicesPerJobCount) where T : struct, IJobParallelForBatch
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForBatchExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.ScheduleParallelFor(ref jobScheduleParameters, arrayLength, arrayLength);
		}

		public static void RunBatch<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBatch
		{
			jobData.Run(arrayLength, arrayLength);
		}

		public static void RunBatchByRef<T>(this T jobData, int arrayLength) where T : struct, IJobParallelForBatch
		{
			(ref jobData).RunByRef<T>(arrayLength, arrayLength);
		}

		internal struct JobParallelForBatchProducer<T> where T : struct, IJobParallelForBatch
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				if (*IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.jobReflectionData.Data == IntPtr.Zero)
				{
					*IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(T), new IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.ExecuteJobFunction(IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>.Execute), null, null);
				}
			}

			public static void Execute(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				int num;
				int num2;
				while (JobsUtility.GetWorkStealingRange(ref ranges, jobIndex, out num, out num2))
				{
					jobData.Execute(num, num2 - num);
				}
			}

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<IJobParallelForBatchExtensions.JobParallelForBatchProducer<T>>(0U);

			internal delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
		}
	}
}
