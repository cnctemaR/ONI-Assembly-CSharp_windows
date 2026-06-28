using System;
using UnityEngine;

public class RoomTypeCategory : Resource
{
	public RoomTypeCategory(string id, string name, Color color)
		: base(id, name)
	{
		this.color = color;
	}

	public Color color { get; private set; }
}
