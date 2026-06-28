using System;
using System.Collections.Generic;

public class LayerGradient : Gradient<List<string>>
{
	public LayerGradient()
		: base(null, 0f)
	{
	}

	public LayerGradient(List<string> content, float bandSize)
		: base(content, bandSize)
	{
	}
}
