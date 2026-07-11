using System;
using System.Runtime.InteropServices;

namespace Klei
{
	public class KProfilerPlugin
	{
		[DllImport("KProfilerPlugin")]
		public static extern void StartProfiling();

		[DllImport("KProfilerPlugin")]
		public static extern bool IsProfiling();

		[DllImport("KProfilerPlugin")]
		public static extern void StopProfiling();

		[DllImport("KProfilerPlugin")]
		public static extern void BeginFrame();

		[DllImport("KProfilerPlugin")]
		public static extern void EndFrame();

		[DllImport("KProfilerPlugin")]
		public static extern ulong GetThreadID();

		[DllImport("KProfilerPlugin")]
		public static extern void SetThreadName(ulong threadId, string name, string category);

		[DllImport("KProfilerPlugin")]
		public static extern void Begin(string eventName, string category);

		[DllImport("KProfilerPlugin")]
		public static extern void End();

		[DllImport("KProfilerPlugin")]
		public static extern bool IsAppAvailable();

		private const string Import = "KProfilerPlugin";
	}
}
