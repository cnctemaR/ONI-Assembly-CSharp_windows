using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Experimental.Animations
{
	internal struct ProcessAnimationJobStruct<T> where T : struct, IAnimationJob
	{
		public static IntPtr GetJobReflectionData()
		{
			if (ProcessAnimationJobStruct<T>.jobReflectionData == IntPtr.Zero)
			{
				ProcessAnimationJobStruct<T>.jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.Single, new ProcessAnimationJobStruct<T>.ExecuteJobFunction(ProcessAnimationJobStruct<T>.ExecuteProcessRootMotion), new ProcessAnimationJobStruct<T>.ExecuteJobFunction(ProcessAnimationJobStruct<T>.ExecuteProcessAnimation), null);
			}
			return ProcessAnimationJobStruct<T>.jobReflectionData;
		}

		public unsafe static void ExecuteProcessAnimation(ref T data, IntPtr animationStreamPtr, IntPtr unusedPtr, ref JobRanges ranges, int jobIndex)
		{
			AnimationStream animationStream;
			UnsafeUtility.CopyPtrToStructure<AnimationStream>((void*)animationStreamPtr, out animationStream);
			data.ProcessAnimation(animationStream);
		}

		public unsafe static void ExecuteProcessRootMotion(ref T data, IntPtr animationStreamPtr, IntPtr unusedPtr, ref JobRanges ranges, int jobIndex)
		{
			AnimationStream animationStream;
			UnsafeUtility.CopyPtrToStructure<AnimationStream>((void*)animationStreamPtr, out animationStream);
			data.ProcessRootMotion(animationStream);
		}

		private static IntPtr jobReflectionData;

		public delegate void ExecuteJobFunction(ref T data, IntPtr animationStreamPtr, IntPtr unusedPtr, ref JobRanges ranges, int jobIndex);
	}
}
