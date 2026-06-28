using System;
using System.Diagnostics;
using UnityEngine;

public static class KProfiler
{
	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string name)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string name, global::UnityEngine.Object target)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSample()
	{
	}
}
