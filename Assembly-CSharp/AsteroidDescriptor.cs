using System;
using System.Collections.Generic;
using UnityEngine;

public struct AsteroidDescriptor
{
	public AsteroidDescriptor(string text, string tooltip, List<global::Tuple<string, Color, float>> bands = null)
	{
		this.text = text;
		this.tooltip = tooltip;
		this.bands = bands;
	}

	public string text;

	public string tooltip;

	public List<global::Tuple<string, Color, float>> bands;
}
