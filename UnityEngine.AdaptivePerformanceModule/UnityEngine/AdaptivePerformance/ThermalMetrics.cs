using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct ThermalMetrics
	{
		public WarningLevel WarningLevel { readonly get; set; }

		public float TemperatureLevel { readonly get; set; }

		public float TemperatureTrend { readonly get; set; }
	}
}
