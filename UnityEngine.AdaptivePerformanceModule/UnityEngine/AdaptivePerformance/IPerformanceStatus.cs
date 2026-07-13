using System;

namespace UnityEngine.AdaptivePerformance
{
	public interface IPerformanceStatus
	{
		PerformanceMetrics PerformanceMetrics { get; }

		FrameTiming FrameTiming { get; }

		event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;

		event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;

		event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;

		PerformanceMode PerformanceMode { get; }
	}
}
