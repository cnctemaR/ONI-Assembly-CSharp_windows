using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct FrameTiming
	{
		public float CurrentFrameTime { readonly get; set; }

		public float AverageFrameTime { readonly get; set; }

		public float CurrentGpuFrameTime { readonly get; set; }

		public float AverageGpuFrameTime { readonly get; set; }

		public float CurrentCpuFrameTime { readonly get; set; }

		public float AverageCpuFrameTime { readonly get; set; }
	}
}
