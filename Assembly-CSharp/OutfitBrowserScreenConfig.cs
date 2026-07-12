using System;
using UnityEngine;

public readonly struct OutfitBrowserScreenConfig
{
	public OutfitBrowserScreenConfig(Option<ClothingOutfitTarget> selectedTarget, Option<Personality> minionPersonality, Option<GameObject> minionInstance)
	{
		this.selectedTarget = selectedTarget;
		this.minionPersonality = minionPersonality;
		this.isPickingOutfitForDupe = minionPersonality.HasValue || minionInstance.HasValue;
		this.targetMinionInstance = minionInstance;
		this.isValid = true;
	}

	public OutfitBrowserScreenConfig WithOutfit(Option<ClothingOutfitTarget> sourceTarget)
	{
		return new OutfitBrowserScreenConfig(sourceTarget, this.minionPersonality, this.targetMinionInstance);
	}

	public string GetMinionName()
	{
		if (this.targetMinionInstance.HasValue)
		{
			return this.targetMinionInstance.Value.GetProperName();
		}
		if (this.minionPersonality.HasValue)
		{
			return this.minionPersonality.Value.Name;
		}
		return "-";
	}

	public static OutfitBrowserScreenConfig Mannequin()
	{
		return new OutfitBrowserScreenConfig(Option.None, Option.None, Option.None);
	}

	public static OutfitBrowserScreenConfig Minion(Personality personality)
	{
		return new OutfitBrowserScreenConfig(Option.None, personality, Option.None);
	}

	public static OutfitBrowserScreenConfig Minion(GameObject minionInstance)
	{
		Personality personality = Db.Get().Personalities.Get(minionInstance.GetComponent<MinionIdentity>().personalityResourceId);
		return new OutfitBrowserScreenConfig(ClothingOutfitTarget.FromMinion(minionInstance), personality, minionInstance);
	}

	public void ApplyAndOpenScreen()
	{
		LockerNavigator.Instance.outfitBrowserScreen.GetComponent<OutfitBrowserScreen>().Configure(this);
		LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitBrowserScreen);
	}

	public readonly Option<ClothingOutfitTarget> selectedTarget;

	public readonly Option<Personality> minionPersonality;

	public readonly Option<GameObject> targetMinionInstance;

	public readonly bool isValid;

	public readonly bool isPickingOutfitForDupe;
}
