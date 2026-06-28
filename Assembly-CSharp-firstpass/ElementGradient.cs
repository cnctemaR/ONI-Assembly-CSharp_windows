using System;
using ProcGen;

public class ElementGradient : Gradient<string>
{
	public ElementGradient()
		: base(null, 0f)
	{
	}

	public ElementGradient(string content, float bandSize, SampleDescriber.Override overrides)
		: base(content, bandSize)
	{
		this.overrides = overrides;
	}

	public SampleDescriber.Override overrides { get; set; }
}
