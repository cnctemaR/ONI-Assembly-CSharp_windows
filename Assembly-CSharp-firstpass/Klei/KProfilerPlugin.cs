using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Klei
{
	public class KProfilerPlugin
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void InitModule()
		{
		}

		[DllImport("SimDLL")]
		public static extern void kprofiler_load_plugin();

		[DllImport("SimDLL")]
		public static extern void kprofiler_unload_plugin();

		[DllImport("SimDLL")]
		public static extern void kprofiler_start_http_control_listener(int port);

		[DllImport("SimDLL")]
		public static extern void kprofiler_start_http_data_sender(int port);

		[DllImport("SimDLL")]
		public static extern void kprofiler_start_file_data_sender(string filename);

		[DllImport("SimDLL")]
		public static extern void kprofiler_flush_data_sender();

		[DllImport("SimDLL")]
		public static extern void kprofiler_stop_data_sender();

		[DllImport("SimDLL")]
		public static extern void kprofiler_start_profiling();

		[DllImport("SimDLL")]
		public static extern void kprofiler_stop_profiling(int broadcast_info);

		[DllImport("SimDLL")]
		public static extern ulong kprofile_record_string(string str);

		[DllImport("SimDLL")]
		public static extern ulong kprofiler_get_thread_uid();

		[DllImport("SimDLL")]
		public static extern void kprofiler_set_thread_info(ulong thread_id, ulong name, ulong category);

		[DllImport("SimDLL")]
		public static extern void kprofiler_begin_section(ulong name, ulong category, long gcAllocCount);

		[DllImport("SimDLL")]
		public static extern void kprofiler_end_section(long gcAllocCount);

		[DllImport("SimDLL")]
		public static extern void kprofiler_ping(ulong name, ulong category, double value);

		[DllImport("SimDLL")]
		public static extern void kprofiler_counter(ulong name, double value);

		private const string Import = "KProfilerPlugin";

		public static bool Initialized;
	}
}
