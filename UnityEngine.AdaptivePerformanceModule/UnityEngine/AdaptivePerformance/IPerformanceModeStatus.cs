using System;

namespace UnityEngine.AdaptivePerformance
{
	public interface IPerformanceModeStatus
	{
		PerformanceMode PerformanceMode { get; }

		event PerformanceModeEventHandler PerformanceModeEvent;
	}
}
