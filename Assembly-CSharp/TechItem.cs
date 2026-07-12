using System;
using UnityEngine;

public class TechItem : Resource
{
	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, bool, Sprite> getUISprite, string parentTechId, string[] dlcIds, bool isPOIUnlock = false)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTechId = parentTechId;
		this.dlcIds = dlcIds;
		this.isPOIUnlock = isPOIUnlock;
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
		return this.ParentTech.IsComplete() || this.IsPOIUnlocked();
	}

	private bool IsPOIUnlocked()
	{
		if (this.isPOIUnlock)
		{
			TechInstance techInstance = Research.Instance.Get(this.ParentTech);
			if (techInstance != null)
			{
				return techInstance.UnlockedPOITechIds.Contains(this.Id);
			}
		}
		return false;
	}

	public void POIUnlocked()
	{
		DebugUtil.DevAssert(this.isPOIUnlock, "Trying to unlock tech item " + this.Id + " via POI and it's not marked as POI unlockable.", null);
		if (this.isPOIUnlock && !this.IsComplete())
		{
			Research.Instance.Get(this.ParentTech).UnlockPOITech(this.Id);
		}
	}

	public string description;

	public Func<string, bool, Sprite> getUISprite;

	public string parentTechId;

	public string[] dlcIds;

	public bool isPOIUnlock;
}
