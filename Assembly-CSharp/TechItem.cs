using System;
using UnityEngine;

public class TechItem : Resource
{
	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, Sprite> getUISprite, Tech parentTech)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTech = parentTech;
	}

	public Sprite UISprite()
	{
		return this.getUISprite("ui");
	}

	public bool IsComplete()
	{
		return this.parentTech.IsComplete();
	}

	public string description;

	public Func<string, Sprite> getUISprite;

	public Tech parentTech;
}
