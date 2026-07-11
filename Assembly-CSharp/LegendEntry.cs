using System;
using UnityEngine;

public class LegendEntry
{
	public LegendEntry(string name, string desc, Color colour)
	{
		this.name = name;
		this.desc = desc;
		this.colour = colour;
	}

	public string name;

	public string desc;

	public Color colour;
}
