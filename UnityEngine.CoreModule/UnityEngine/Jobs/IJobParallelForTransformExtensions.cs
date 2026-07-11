using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Jobs
{
	/// <summary>
	///   <para>Extension methods for IJobParallelForTransform.</para>
	/// </summary>
	public static class IJobParallelForTransformExtensions
	{
		public static JobHandle Schedule<T>(this T jobData, TransformAccessArray transforms, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelForTransform
		{
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<T>(ref jobData), IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.Initialize(), dependsOn, ScheduleMode.Batched);
			return JobsUtility.ScheduleParallelForTransform(ref jobScheduleParameters, transforms.GetTransformAccessArrayForSchedule());
		}

		internal struct TransformParallelForLoopStruct<T> where T : struct, IJobParallelForTransform
		{
			public static IntPtr Initialize()
			{
				if (IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData == IntPtr.Zero)
				{
					IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor, new IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.ExecuteJobFunction(IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.Execute), null, null);
				}
				return IJobParallelForTransformExtensions.TransformParallelForLoopStruct<T>.jobReflectionData;
			}

			public unsafe static void Execute(ref T jobData, IntPtr jobData2, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex)
			{
				IntPtr intPtr;
				UnsafeUtility.CopyPtrToStructure<IntPtr>((void*)jobData2, out intPtr);
				int* ptr = (int*)(void*)TransformAccessArray.GetSortedToUserIndex(intPtr);
				TransformAccess* ptr2 = (TransformAccess*)(void*)TransformAccessArray.GetSortedTransformAccess(intPtr);
				int num;
				int num2;
				JobsUtility.GetJobRange(ref ranges, jobIndex, out num, out num2);
				for (int i = num; i < num2; i++)
				{
					int num3 = i;
					int num4 = ptr[num3];
					jobData.Execute(num4, ptr2[num3]);
				}
			}

			public static IntPtr jobReflectionData;

			public delegate void ExecuteJobFunction(ref T jobData, IntPtr additionalPtr, IntPtr bufferRangePatchData, ref JobRanges ranges, int jobIndex);
		}
	}
}
