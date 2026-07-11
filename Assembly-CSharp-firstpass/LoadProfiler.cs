using System;
using System.Diagnostics;
using UnityEngine;

public class LoadProfiler : ProfilerBase
{
	private LoadProfiler(string file_prefix)
		: base(file_prefix)
	{
	}

	public static LoadProfiler Instance
	{
		get
		{
			if (LoadProfiler.instance == null)
			{
				LoadProfiler.instance = new LoadProfiler("load_stats_");
				if (!Stopwatch.IsHighResolution)
				{
					global::UnityEngine.Debug.LogWarning("Low resolution timer! [" + Stopwatch.Frequency + "] ticks per second");
				}
			}
			return LoadProfiler.instance;
		}
	}

	private static void ProfilerSection(string region_name, string file = "unknown", uint line = 0U)
	{
		LoadProfiler.Instance.Push(region_name, file, line);
	}

	private static void EndProfilerSection()
	{
		LoadProfiler.Instance.Pop();
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void AddEvent(string event_name, string file = "unknown", uint line = 0U)
	{
		if (!LoadProfiler.Instance.IsRecording() || LoadProfiler.Instance.proFile == null)
		{
			return;
		}
		LoadProfiler.Instance.ManifestThreadInfo(null).WriteLine("GAME", event_name, LoadProfiler.Instance.sw, "I", "},");
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void BeginSample(string region_name)
	{
		LoadProfiler.Instance.Push(region_name, "unknown", 0U);
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void EndSample()
	{
		LoadProfiler.Instance.Pop();
	}

	private static LoadProfiler instance;
}
