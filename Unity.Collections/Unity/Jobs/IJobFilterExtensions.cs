using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Jobs
{
	public static class IJobFilterExtensions
	{
		public static void EarlyJobInit<T>() where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.Initialize();
		}

		private unsafe static IntPtr GetReflectionData<T>() where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.Initialize();
			return *IJobFilterExtensions.JobFilterProducer<T>.jobReflectionData.Data;
		}

		public static JobHandle ScheduleAppend<T>(this T jobData, NativeList<int> indices, int arrayLength, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobFilter
		{
			return (ref jobData).ScheduleAppendByRef<T>(indices, arrayLength, dependsOn);
		}

		public static JobHandle ScheduleFilter<T>(this T jobData, NativeList<int> indices, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobFilter
		{
			return (ref jobData).ScheduleFilterByRef<T>(indices, dependsOn);
		}

		public static void RunAppend<T>(this T jobData, NativeList<int> indices, int arrayLength) where T : struct, IJobFilter
		{
			(ref jobData).RunAppendByRef<T>(indices, arrayLength);
		}

		public static void RunFilter<T>(this T jobData, NativeList<int> indices) where T : struct, IJobFilter
		{
			(ref jobData).RunFilterByRef<T>(indices);
		}

		public static JobHandle ScheduleAppendByRef<T>(this T jobData, NativeList<int> indices, int arrayLength, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper = new IJobFilterExtensions.JobFilterProducer<T>.JobWrapper
			{
				JobData = jobData,
				outputIndices = indices,
				appendCount = arrayLength
			};
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<IJobFilterExtensions.JobFilterProducer<T>.JobWrapper>(ref jobWrapper), IJobFilterExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Single);
			return JobsUtility.Schedule(ref jobScheduleParameters);
		}

		public static JobHandle ScheduleFilterByRef<T>(this T jobData, NativeList<int> indices, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper = new IJobFilterExtensions.JobFilterProducer<T>.JobWrapper
			{
				JobData = jobData,
				outputIndices = indices,
				appendCount = -1
			};
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<IJobFilterExtensions.JobFilterProducer<T>.JobWrapper>(ref jobWrapper), IJobFilterExtensions.GetReflectionData<T>(), dependsOn, ScheduleMode.Single);
			return JobsUtility.Schedule(ref jobScheduleParameters);
		}

		public static void RunAppendByRef<T>(this T jobData, NativeList<int> indices, int arrayLength) where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper = new IJobFilterExtensions.JobFilterProducer<T>.JobWrapper
			{
				JobData = jobData,
				outputIndices = indices,
				appendCount = arrayLength
			};
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<IJobFilterExtensions.JobFilterProducer<T>.JobWrapper>(ref jobWrapper), IJobFilterExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.Schedule(ref jobScheduleParameters);
		}

		public static void RunFilterByRef<T>(this T jobData, NativeList<int> indices) where T : struct, IJobFilter
		{
			IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper = new IJobFilterExtensions.JobFilterProducer<T>.JobWrapper
			{
				JobData = jobData,
				outputIndices = indices,
				appendCount = -1
			};
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<IJobFilterExtensions.JobFilterProducer<T>.JobWrapper>(ref jobWrapper), IJobFilterExtensions.GetReflectionData<T>(), default(JobHandle), ScheduleMode.Run);
			JobsUtility.Schedule(ref jobScheduleParameters);
		}

		internal struct JobFilterProducer<T> where T : struct, IJobFilter
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				if (*IJobFilterExtensions.JobFilterProducer<T>.jobReflectionData.Data == IntPtr.Zero)
				{
					*IJobFilterExtensions.JobFilterProducer<T>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(IJobFilterExtensions.JobFilterProducer<T>.JobWrapper), typeof(T), new IJobFilterExtensions.JobFilterProducer<T>.ExecuteJobFunction(IJobFilterExtensions.JobFilterProducer<T>.Execute));
				}
			}

			public static void Execute(ref IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				if (jobWrapper.appendCount == -1)
				{
					IJobFilterExtensions.JobFilterProducer<T>.ExecuteFilter(ref jobWrapper, bufferRangePatchData);
					return;
				}
				IJobFilterExtensions.JobFilterProducer<T>.ExecuteAppend(ref jobWrapper, bufferRangePatchData);
			}

			public unsafe static void ExecuteAppend(ref IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper, IntPtr bufferRangePatchData)
			{
				int length = jobWrapper.outputIndices.Length;
				jobWrapper.outputIndices.Capacity = math.max(jobWrapper.appendCount + length, jobWrapper.outputIndices.Capacity);
				int* unsafePtr = jobWrapper.outputIndices.GetUnsafePtr<int>();
				int num = length;
				for (int num2 = 0; num2 != jobWrapper.appendCount; num2++)
				{
					if (jobWrapper.JobData.Execute(num2))
					{
						unsafePtr[num] = num2;
						num++;
					}
				}
				jobWrapper.outputIndices.ResizeUninitialized(num);
			}

			public unsafe static void ExecuteFilter(ref IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper, IntPtr bufferRangePatchData)
			{
				int* unsafePtr = jobWrapper.outputIndices.GetUnsafePtr<int>();
				int length = jobWrapper.outputIndices.Length;
				int num = 0;
				for (int num2 = 0; num2 != length; num2++)
				{
					int num3 = unsafePtr[num2];
					if (jobWrapper.JobData.Execute(num3))
					{
						unsafePtr[num] = num3;
						num++;
					}
				}
				jobWrapper.outputIndices.ResizeUninitialized(num);
			}

			internal static readonly SharedStatic<IntPtr> jobReflectionData = SharedStatic<IntPtr>.GetOrCreate<IJobFilterExtensions.JobFilterProducer<T>>(0U);

			public struct JobWrapper
			{
				[NativeDisableParallelForRestriction]
				public NativeList<int> outputIndices;

				public int appendCount;

				public T JobData;
			}

			public delegate void ExecuteJobFunction(ref IJobFilterExtensions.JobFilterProducer<T>.JobWrapper jobWrapper, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
		}
	}
}
