using System;
using System.Diagnostics;
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/Sampler.bindings.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Profiler/Marker.h")]
	public sealed class CustomSampler : Sampler
	{
		internal CustomSampler()
		{
		}

		internal CustomSampler(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public static CustomSampler Create(string name, bool collectGpuData = false)
		{
			IntPtr intPtr = ProfilerUnsafeUtility.CreateMarker(name, 1, MarkerFlags.AvailabilityNonDevelopment | (collectGpuData ? MarkerFlags.SampleGPU : MarkerFlags.Default), 0);
			bool flag = intPtr == IntPtr.Zero;
			CustomSampler customSampler;
			if (flag)
			{
				customSampler = CustomSampler.s_InvalidCustomSampler;
			}
			else
			{
				customSampler = new CustomSampler(intPtr);
			}
			return customSampler;
		}

		[Conditional("ENABLE_PROFILER")]
		[IgnoredByDeepProfiler]
		public void Begin()
		{
			ProfilerUnsafeUtility.BeginSample(this.m_Ptr);
		}

		[IgnoredByDeepProfiler]
		[Conditional("ENABLE_PROFILER")]
		public void Begin(Object targetObject)
		{
			ProfilerUnsafeUtility.Internal_BeginWithObject(this.m_Ptr, targetObject);
		}

		[IgnoredByDeepProfiler]
		[Conditional("ENABLE_PROFILER")]
		public void End()
		{
			ProfilerUnsafeUtility.EndSample(this.m_Ptr);
		}

		internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();
	}
}
