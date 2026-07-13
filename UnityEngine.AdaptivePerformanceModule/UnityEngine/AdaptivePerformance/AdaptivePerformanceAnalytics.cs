using System;
using System.Diagnostics;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance
{
	internal static class AdaptivePerformanceAnalytics
	{
		[Conditional("UNITY_ANALYTICS")]
		public static void RegisterFeature(string feature, bool status)
		{
		}

		[Conditional("UNITY_ANALYTICS")]
		public static void SendAdaptiveStartupEvent(AdaptivePerformanceSubsystem subsystem)
		{
		}

		[Conditional("UNITY_ANALYTICS")]
		public static void SendAdaptiveFeatureUpdateEvent(string feature, bool status)
		{
		}

		[Conditional("UNITY_ANALYTICS")]
		public static void SendAdaptivePerformanceThermalEvent(ThermalMetrics thermalMetrics)
		{
		}

		internal static class AnalyticsLog
		{
			[Conditional("ADAPTIVE_PERFORMANCE_ANALYTICS_LOGGING")]
			public static void Debug(string format, params object[] args)
			{
			}
		}
	}
}
