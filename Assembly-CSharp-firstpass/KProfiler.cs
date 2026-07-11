using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public static class KProfiler
{
	public static void BeginThreadProfiling(string threadGroupName, string threadName)
	{
	}

	public static void EndThreadProfiling()
	{
	}

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

	public static void AddEvent(string event_name)
	{
	}

	public static void AddCounter(string event_name, List<KeyValuePair<string, int>> series_name_counts)
	{
	}

	public static void AddCounter(string event_name, string series_name, int count)
	{
	}

	public static void AddCounter(string event_name, int count)
	{
		KProfiler.AddCounter(event_name, event_name, count);
	}

	public static int counter;

	public static Thread main_thread;

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
