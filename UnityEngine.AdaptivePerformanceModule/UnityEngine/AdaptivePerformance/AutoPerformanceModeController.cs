using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class AutoPerformanceModeController
	{
		public AutoPerformanceModeController(IPerformanceModeStatus perfModeStat)
		{
			perfModeStat.PerformanceModeEvent += delegate(PerformanceMode mode)
			{
				this.OnPerformanceModeChange(mode);
			};
		}

		private void OnPerformanceModeChange(PerformanceMode performanceMode)
		{
			if (performanceMode != PerformanceMode.Optimize)
			{
				if (performanceMode != PerformanceMode.Battery)
				{
					Application.targetFrameRate = -1;
				}
				else
				{
					Application.targetFrameRate = 30;
				}
			}
			else
			{
				Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
			}
			APLog.Debug(string.Format("[AutoPerformanceModeController] Performance Mode: {0}, fps: {1}", performanceMode, Application.targetFrameRate), Array.Empty<object>());
		}

		private string m_FeatureName = "Auto Performance Mode Control";
	}
}
