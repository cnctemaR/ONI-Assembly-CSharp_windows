using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class AdaptivePerformanceScalerEfficiencyTracker
	{
		public bool IsRunning
		{
			get
			{
				return this.m_Scaler != null;
			}
		}

		public void Start(AdaptivePerformanceScaler scaler, bool isApply)
		{
			Debug.Assert(!this.IsRunning, "AdaptivePerformanceScalerEfficiencyTracker is already running");
			this.m_Scaler = scaler;
			this.m_LastAverageGpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime;
			this.m_LastAverageCpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime;
			this.m_IsApplied = true;
		}

		public void Stop()
		{
			float num = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime - this.m_LastAverageGpuFrameTime;
			float num2 = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime - this.m_LastAverageCpuFrameTime;
			int num3 = (this.m_IsApplied ? 1 : (-1));
			this.m_Scaler.GpuImpact = num3 * (int)(num * 1000f);
			this.m_Scaler.CpuImpact = num3 * (int)(num2 * 1000f);
			this.m_Scaler = null;
		}

		private AdaptivePerformanceScaler m_Scaler;

		private float m_LastAverageGpuFrameTime;

		private float m_LastAverageCpuFrameTime;

		private bool m_IsApplied;
	}
}
