using System;

public class ElementGradient : Gradient<SimHashes>
{
	public ElementGradient()
		: base((SimHashes)0, 0f)
	{
	}

	public ElementGradient(SimHashes content, float bandSize)
		: base(content, bandSize)
	{
	}
}
