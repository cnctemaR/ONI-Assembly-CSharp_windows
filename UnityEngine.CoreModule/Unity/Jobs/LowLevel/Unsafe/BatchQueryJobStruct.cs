using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	public struct BatchQueryJobStruct<T> where T : struct
	{
		public static IntPtr Initialize()
		{
			if (BatchQueryJobStruct<T>.jobReflectionData == IntPtr.Zero)
			{
				BatchQueryJobStruct<T>.jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor, null, null, null);
			}
			return BatchQueryJobStruct<T>.jobReflectionData;
		}

		internal static IntPtr jobReflectionData;
	}
}
