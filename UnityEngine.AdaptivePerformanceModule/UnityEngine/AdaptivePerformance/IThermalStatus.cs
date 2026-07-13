using System;

namespace UnityEngine.AdaptivePerformance
{
	public interface IThermalStatus
	{
		ThermalMetrics ThermalMetrics { get; }

		event ThermalEventHandler ThermalEvent;
	}
}
