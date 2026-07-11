using System;
using System.Diagnostics;
using UnityEngine;

public static class KProfiler
{
	public static int BeginSampleI(string region_name)
	{
		int num = KProfiler.counter;
		KProfiler.counter++;
		return num;
	}

	public static int BeginSampleI(string region_name, global::UnityEngine.Object profiler_obj)
	{
		int num = KProfiler.counter;
		KProfiler.counter++;
		return num;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name)
	{
		KProfiler.BeginSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, int count)
	{
		KProfiler.BeginSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, global::UnityEngine.Object profiler_obj)
	{
		KProfiler.BeginSampleI(region_name, profiler_obj);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSample()
	{
		KProfiler.EndSampleI();
	}

	public static int EndSampleI()
	{
		KProfiler.counter--;
		return KProfiler.counter;
	}

	public static int counter;

	public struct Region : IDisposable
	{
		public Region(string region_name, global::UnityEngine.Object profiler_obj = null)
		{
		}

		public void Dispose()
		{
		}
	}
}
