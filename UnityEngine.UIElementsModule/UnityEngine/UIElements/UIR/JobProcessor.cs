using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.UIR
{
	[NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRendererJobProcessor.h")]
	internal static class JobProcessor
	{
		internal static JobHandle ScheduleNudgeJobs(IntPtr buffer, int jobCount)
		{
			JobHandle jobHandle;
			JobProcessor.ScheduleNudgeJobs_Injected(buffer, jobCount, out jobHandle);
			return jobHandle;
		}

		internal static JobHandle ScheduleConvertMeshJobs(IntPtr buffer, int jobCount)
		{
			JobHandle jobHandle;
			JobProcessor.ScheduleConvertMeshJobs_Injected(buffer, jobCount, out jobHandle);
			return jobHandle;
		}

		internal static JobHandle ScheduleCopyMeshJobs(IntPtr buffer, int jobCount)
		{
			JobHandle jobHandle;
			JobProcessor.ScheduleCopyMeshJobs_Injected(buffer, jobCount, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleNudgeJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleConvertMeshJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleCopyMeshJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);
	}
}
