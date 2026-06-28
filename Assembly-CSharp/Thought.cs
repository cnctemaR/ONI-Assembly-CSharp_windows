using System;
using UnityEngine;

public class Thought : Resource
{
	public Thought(string id, ResourceSet parent, string icon, LocString hover_text, bool show_immediately = false)
		: base(id, parent, null)
	{
		this.texture = Assets.GetTexture(icon);
		this.showImmediately = show_immediately;
		this.hoverText = hover_text;
	}

	public int priority;

	public Texture2D texture;

	public bool showImmediately;

	public LocString hoverText;
}
