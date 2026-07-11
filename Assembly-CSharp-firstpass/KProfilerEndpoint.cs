using System;
using System.Diagnostics;

public class KProfilerEndpoint
{
	[Conditional("ENABLE_KPROFILER")]
	public virtual void Begin(string name)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void End(string name)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void BeginFrame()
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void EndFrame()
	{
	}
}
