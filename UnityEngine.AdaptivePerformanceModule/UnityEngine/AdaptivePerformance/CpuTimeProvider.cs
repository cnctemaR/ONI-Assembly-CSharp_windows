using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class CpuTimeProvider
	{
		public float CpuFrameTime
		{
			get
			{
				bool flag = this.GetLatestTimings() >= 1U;
				if (flag)
				{
					double num = this.m_FrameTimings[0].cpuMainThreadFrameTime + this.m_FrameTimings[0].cpuRenderThreadFrameTime;
					bool flag2 = num > 0.0;
					if (flag2)
					{
						return (float)(num * 0.001);
					}
				}
				return -1f;
			}
		}

		protected virtual uint GetLatestTimings()
		{
			return FrameTimingManager.GetLatestTimings(1U, this.m_FrameTimings);
		}

		public void Measure()
		{
			FrameTimingManager.CaptureFrameTimings();
		}

		private FrameTiming[] m_FrameTimings = new FrameTiming[1];
	}
}
