using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Profiling;
using UnityEngine.Profiling;

namespace UnityEngine.AdaptivePerformance
{
	public static class AdaptivePerformanceProfilerStats
	{
		[Conditional("ENABLE_PROFILER")]
		public unsafe static void EmitScalerDataToProfilerStream(string scalerName, bool enabled, int overrideLevel, int currentLevel, float scale, bool applied, int maxLevel)
		{
			bool flag = !Profiler.enabled || scalerName.Length == 0;
			if (!flag)
			{
				AdaptivePerformanceProfilerStats.ScalerInfo scalerInfo;
				bool flag2 = AdaptivePerformanceProfilerStats.scalerInfos.TryGetValue(scalerName, out scalerInfo);
				bool flag3 = !flag2;
				if (flag3)
				{
					scalerInfo = default(AdaptivePerformanceProfilerStats.ScalerInfo);
				}
				scalerInfo.enabled = (enabled ? 1U : 0U);
				scalerInfo.overrideLevel = overrideLevel;
				scalerInfo.currentLevel = currentLevel;
				scalerInfo.scale = scale;
				scalerInfo.maxLevel = maxLevel;
				scalerInfo.applied = (applied ? 1U : 0U);
				Encoding.ASCII.GetBytes(scalerName.AsSpan(), new Span<byte>((void*)(&scalerInfo.scalerName.FixedElementField), 320));
				bool flag4 = !flag2;
				if (flag4)
				{
					AdaptivePerformanceProfilerStats.scalerInfos.Add(scalerName, scalerInfo);
				}
				else
				{
					AdaptivePerformanceProfilerStats.scalerInfos[scalerName] = scalerInfo;
				}
			}
		}

		public static void FlushScalerDataToProfilerStream()
		{
			AdaptivePerformanceProfilerStats.ScalerInfo[] array = new AdaptivePerformanceProfilerStats.ScalerInfo[AdaptivePerformanceProfilerStats.scalerInfos.Count];
			AdaptivePerformanceProfilerStats.scalerInfos.Values.CopyTo(array, 0);
		}

		public static readonly ProfilerCategory AdaptivePerformanceProfilerCategory = ProfilerCategory.Scripts;

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> CurrentCPUMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("CPU frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> AvgCPUMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("CPU avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> CurrentGPUMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("GPU frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> AvgGPUMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("GPU avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<int> CurrentCPULevelMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<int>("CPU performance level", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<int> CurrentGPULevelMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<int>("GPU performance level", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> CurrentFrametimeMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("Frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> AvgFrametimeMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("Avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<int> WarningLevelMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<int>("Thermal Warning Level", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> TemperatureLevelMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("Temperature Level", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<float> TemperatureTrendMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<float>("Temperature Trend", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<int> BottleneckMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<int>("Bottleneck", ProfilerMarkerDataUnit.Count);

		public static AdaptivePerformanceProfilerStats.CustomProfilerMarker<int> PerformanceModeMarker = new AdaptivePerformanceProfilerStats.CustomProfilerMarker<int>("Performance Mode", ProfilerMarkerDataUnit.Count);

		public static readonly Guid kAdaptivePerformanceProfilerModuleGuid = new Guid("42c5aeb7-fb77-4172-a384-34063f1bd332");

		public static readonly int kScalerDataTag = 0;

		private static Dictionary<string, AdaptivePerformanceProfilerStats.ScalerInfo> scalerInfos = new Dictionary<string, AdaptivePerformanceProfilerStats.ScalerInfo>();

		public readonly struct CustomProfilerMarker<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
		{
			public CustomProfilerMarker(string name, ProfilerMarkerDataUnit dataUnit)
			{
			}

			public void Sample(T value)
			{
			}

			private static byte GetProfilerMarkerDataType()
			{
				switch (Type.GetTypeCode(typeof(T)))
				{
				case TypeCode.Int32:
					return 2;
				case TypeCode.UInt32:
					return 3;
				case TypeCode.Int64:
					return 4;
				case TypeCode.UInt64:
					return 5;
				case TypeCode.Single:
					return 6;
				case TypeCode.Double:
					return 7;
				case TypeCode.String:
					return 9;
				}
				throw new ArgumentException(string.Format("Type {0} is unsupported by ProfilerCounter.", typeof(T)));
			}
		}

		public struct ScalerInfo
		{
			[FixedBuffer(typeof(byte), 320)]
			public AdaptivePerformanceProfilerStats.ScalerInfo.<scalerName>e__FixedBuffer scalerName;

			public uint enabled;

			public int overrideLevel;

			public int currentLevel;

			public int maxLevel;

			public float scale;

			public uint applied;

			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, Size = 320)]
			public struct <scalerName>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}
	}
}
