using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.UIR
{
	[NativeHeader("ModuleOverrides/com.unity.ui/Core/Native/Renderer/UIRendererJobProcessor.h")]
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

		internal static JobHandle ScheduleCopyClosingMeshJobs(IntPtr buffer, int jobCount)
		{
			JobHandle jobHandle;
			JobProcessor.ScheduleCopyClosingMeshJobs_Injected(buffer, jobCount, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleNudgeJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleConvertMeshJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScheduleCopyClosingMeshJobs_Injected(IntPtr buffer, int jobCount, out JobHandle ret);
	}
}
