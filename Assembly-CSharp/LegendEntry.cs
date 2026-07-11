using System;
using UnityEngine;

public class LegendEntry
{
	public LegendEntry(string name, string desc, Color colour, string desc_arg = null)
	{
		this.name = name;
		this.desc = desc;
		this.colour = colour;
		this.desc_arg = desc_arg;
	}

	public string name;

	public string desc;

	public string desc_arg;

	public Color colour;
}
