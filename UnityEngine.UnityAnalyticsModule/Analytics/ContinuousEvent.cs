using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/UnityAnalytics/CoreStats/AnalyticsCoreStats.h")]
	[NativeHeader("Modules/UnityAnalytics/ContinuousEvent/Manager.h")]
	internal class ContinuousEvent
	{
		public static void RegisterCollector<T>(string collectorName, Func<T> del) where T : struct, IComparable<T>, IEquatable<T>
		{
			ContinuousEvent.RegisterCollector_Internal(typeof(T).ToString(), collectorName, del);
		}

		public static void SetEventHistogramThresholds<T>(string eventName, int count, T[] data) where T : struct, IComparable<T>, IEquatable<T>
		{
			ContinuousEvent.SetEventHistogramThresholds_Internal(typeof(T).ToString(), eventName, count, data);
		}

		[StaticAccessor("::GetAnalyticsCoreStats().GetContinuousEventManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterCollector_Internal(string type, string collectorName, object collector);

		[StaticAccessor("::GetAnalyticsCoreStats().GetContinuousEventManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEventHistogramThresholds_Internal(string type, string eventName, int count, object data);

		[StaticAccessor("::GetAnalyticsCoreStats().GetContinuousEventManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableEvent(string eventName, bool enabled);

		[StaticAccessor("::GetAnalyticsCoreStats().GetContinuousEventManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ConfigureEvent(string eventName, string collectorName, float interval, float period, bool enabled, bool custom = false);
	}
}
