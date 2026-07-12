using System;
using System.Diagnostics;
using UnityEngine;

public class WorldGenProfiler : ProfilerBase
{
	private WorldGenProfiler(string file_prefix)
		: base(file_prefix)
	{
	}

	public static WorldGenProfiler Instance
	{
		get
		{
			if (WorldGenProfiler.instance == null)
			{
				WorldGenProfiler.instance = new WorldGenProfiler("worldgen_stats_");
				if (!Stopwatch.IsHighResolution)
				{
					global::UnityEngine.Debug.LogWarning("Low resolution timer! [" + Stopwatch.Frequency.ToString() + "] ticks per second");
				}
			}
			return WorldGenProfiler.instance;
		}
	}

	private static void ProfilerSection(string region_name, string file = "unknown", uint line = 0U)
	{
		WorldGenProfiler.Instance.Push(region_name, file, line);
	}

	private static void EndProfilerSection()
	{
		WorldGenProfiler.Instance.Pop();
	}

	[Conditional("ENABLE_WORLDGEN_STATS")]
	public static void AddEvent(string event_name, string file = "unknown", uint line = 0U)
	{
		if (!WorldGenProfiler.Instance.IsRecording() || WorldGenProfiler.Instance.proFile == null)
		{
			return;
		}
		WorldGenProfiler.Instance.ManifestThreadInfo(null).WriteLine("GAME", event_name, WorldGenProfiler.Instance.sw, "I", "},");
	}

	[Conditional("ENABLE_WORLDGEN_STATS")]
	public static void BeginSample(string region_name)
	{
		WorldGenProfiler.Instance.Push(region_name, "unknown", 0U);
	}

	[Conditional("ENABLE_WORLDGEN_STATS")]
	public static void EndSample()
	{
		WorldGenProfiler.Instance.Pop();
	}

	private static WorldGenProfiler instance;
}
