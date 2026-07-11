using System;
using UnityEngine;

public class TechItem : Resource
{
	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, bool, Sprite> getUISprite, Tech parentTech)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTech = parentTech;
	}

	public Sprite UISprite()
	{
		return this.getUISprite("ui", false);
	}

	public bool IsComplete()
	{
		return this.parentTech.IsComplete();
	}

	public string description;

	public Func<string, bool, Sprite> getUISprite;

	public Tech parentTech;
}
