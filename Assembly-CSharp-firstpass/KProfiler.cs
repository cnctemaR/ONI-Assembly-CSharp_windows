using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Klei;
using Unity.Profiling;

public static class KProfiler
{
	private static ulong identify_string(string str)
	{
		ulong num;
		if (KProfiler.known_strings.TryGetValue(str, out num))
		{
			return num;
		}
		ulong num2 = KProfilerPlugin.kprofile_record_string(str);
		KProfiler.known_strings[str] = num2;
		return num2;
	}

	public static void InitProfileRecorders()
	{
	}

	private static long getGCAllocCount()
	{
		return -1L;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void StartProfiling()
	{
		KProfilerPlugin.kprofiler_start_profiling();
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void StopProfiling()
	{
		KProfilerPlugin.kprofiler_stop_profiling(1);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSection(string name, string group = "")
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfiler.identify_string(name);
		KProfiler.identify_string(group);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSection(ulong u_name, ulong u_group)
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_begin_section(u_name, u_group, KProfiler.getGCAllocCount());
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSection(ulong u_name, string group)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSection(string name, ulong u_group)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void BeginDetailedSection(ulong name, ulong group)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void BeginDetailedSection(string name, ulong group)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void BeginDetailedSection(ulong name, string group)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void BeginDetailedSection(string name, string group)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void BeginDetailedSection(string name)
	{
	}

	[Conditional("KPROFILER_DETAILED")]
	public static void EndDetailedSection()
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginFrameSection()
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		ulong num = KProfiler.identify_string("Frame");
		ulong num2 = KProfiler.identify_string("");
		long num3 = -1L;
		KProfilerPlugin.kprofiler_begin_section(num, num2, num3);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSection(string name, int v)
	{
		bool initialized = KProfilerPlugin.Initialized;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSection()
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_end_section(KProfiler.getGCAllocCount());
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndFrameSection()
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSection(string name, int v = 0)
	{
		bool initialized = KProfilerPlugin.Initialized;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void Ping(string name, string group, double value)
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		ulong num = KProfiler.identify_string(name);
		ulong num2 = KProfiler.identify_string(group);
		KProfilerPlugin.kprofiler_ping(num, num2, value);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void Counter(string name, double value)
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_counter(KProfiler.identify_string(name), value);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void FlushData()
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_flush_data_sender();
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void Shutdown()
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_unload_plugin();
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void StreamToFile(string path)
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		KProfilerPlugin.kprofiler_start_file_data_sender(path);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void IdentifyThread(string name, string group)
	{
		if (!KProfilerPlugin.Initialized)
		{
			return;
		}
		ulong num = KProfilerPlugin.kprofiler_get_thread_uid();
		ulong num2 = KProfiler.identify_string(name);
		ulong num3 = KProfiler.identify_string(group);
		KProfilerPlugin.kprofiler_set_thread_info(num, num2, num3);
	}

	private static ProfilerRecorder gcAllocRecorder;

	private static ConcurrentDictionary<string, ulong> known_strings = new ConcurrentDictionary<string, ulong>();
}
