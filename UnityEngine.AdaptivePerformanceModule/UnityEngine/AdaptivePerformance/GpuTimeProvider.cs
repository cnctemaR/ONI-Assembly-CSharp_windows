using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class GpuTimeProvider
	{
		public float GpuFrameTime
		{
			get
			{
				bool flag = this.GetLatestTimings() >= 1U;
				if (flag)
				{
					double gpuFrameTime = this.m_FrameTiming[0].gpuFrameTime;
					bool flag2 = gpuFrameTime > 0.0;
					if (flag2)
					{
						return (float)(gpuFrameTime * 0.001);
					}
				}
				return -1f;
			}
		}

		protected virtual uint GetLatestTimings()
		{
			return FrameTimingManager.GetLatestTimings(1U, this.m_FrameTiming);
		}

		public void Measure()
		{
			FrameTimingManager.CaptureFrameTimings();
		}

		private FrameTiming[] m_FrameTiming = new FrameTiming[1];
	}
}
