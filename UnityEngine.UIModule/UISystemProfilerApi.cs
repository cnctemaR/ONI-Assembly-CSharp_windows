using System;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[IgnoredByDeepProfiler]
	[NativeHeader("Modules/UI/Canvas.h")]
	[StaticAccessor("UI::SystemProfilerApi", StaticAccessorType.DoubleColon)]
	public static class UISystemProfilerApi
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSample(UISystemProfilerApi.SampleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSample(UISystemProfilerApi.SampleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddMarker(string name, Object obj);

		public enum SampleType
		{
			Layout,
			Render
		}
	}
}
