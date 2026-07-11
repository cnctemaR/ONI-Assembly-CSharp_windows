using System;
using Klei;

public class KProfilerPluginEndpoint : KProfilerEndpoint
{
	public override void Begin(string name)
	{
		KProfilerPlugin.Begin(name, "Game");
	}

	public override void End(string name)
	{
		KProfilerPlugin.End();
	}

	public override void BeginFrame()
	{
		KProfilerPlugin.BeginFrame();
	}

	public override void EndFrame()
	{
		KProfilerPlugin.EndFrame();
	}
}
