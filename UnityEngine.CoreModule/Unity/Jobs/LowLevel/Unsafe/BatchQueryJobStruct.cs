using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	public struct BatchQueryJobStruct<T> where T : struct
	{
		public static IntPtr Initialize()
		{
			bool flag = BatchQueryJobStruct<T>.jobReflectionData == IntPtr.Zero;
			if (flag)
			{
				BatchQueryJobStruct<T>.jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), null, null, null);
			}
			return BatchQueryJobStruct<T>.jobReflectionData;
		}

		internal static IntPtr jobReflectionData;
	}
}
