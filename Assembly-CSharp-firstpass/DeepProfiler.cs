using System;
using System.Diagnostics;

public class DeepProfiler
{
	public DeepProfiler(bool enable_profiling)
	{
		this.enableProfiling = enable_profiling;
	}

	[Conditional("UNITY_EDITOR")]
	public void BeginSample(string message)
	{
		if (this.enableProfiling)
		{
		}
	}

	[Conditional("UNITY_EDITOR")]
	public void EndSample()
	{
		if (this.enableProfiling)
		{
		}
	}

	private bool enableProfiling;
}
