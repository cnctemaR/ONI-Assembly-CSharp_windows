using System;
using UnityEngine;

public class TechItem : Resource
{
	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, bool, Sprite> getUISprite, string parentTechId, string[] dlcIds)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTechId = parentTechId;
		this.dlcIds = dlcIds;
	}

	public Tech ParentTech
	{
		get
		{
			return Db.Get().Techs.Get(this.parentTechId);
		}
	}

	public Sprite UISprite()
	{
		return this.getUISprite("ui", false);
	}

	public bool IsComplete()
	{
		return this.ParentTech.IsComplete();
	}

	public string description;

	public Func<string, bool, Sprite> getUISprite;

	public string parentTechId;

	public string[] dlcIds;
}
