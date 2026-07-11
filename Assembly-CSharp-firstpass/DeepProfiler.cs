using System;
using System.Diagnostics;

public class DeepProfiler
{
	public DeepProfiler(bool enable_profiling)
	{
		this.enableProfiling = enable_profiling;
	}

	[Conditional("DEEP_PROFILE")]
	public void BeginSample(string message)
	{
		bool flag = this.enableProfiling;
	}

	[Conditional("DEEP_PROFILE")]
	public void EndSample()
	{
		bool flag = this.enableProfiling;
	}

	private bool enableProfiling;
}
