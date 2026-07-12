using System;
using UnityEngine;

public readonly struct OutfitBrowserScreenConfig
{
	public OutfitBrowserScreenConfig(Option<ClothingOutfitUtility.OutfitType> outfitType, Option<ClothingOutfitTarget> selectedTarget, Option<Personality> minionPersonality, Option<GameObject> minionInstance)
	{
		this.outfitType = outfitType;
		this.selectedTarget = selectedTarget;
		this.minionPersonality = minionPersonality;
		this.isPickingOutfitForDupe = minionPersonality.HasValue || minionInstance.HasValue;
		this.targetMinionInstance = minionInstance;
		this.isValid = true;
		if (minionPersonality.IsSome() || this.targetMinionInstance.IsSome())
		{
			global::Debug.Assert(outfitType.IsSome(), "If viewing outfits for a specific duplicant personality or instance, an outfitType must also be given.");
		}
	}

	public OutfitBrowserScreenConfig WithOutfit(Option<ClothingOutfitTarget> sourceTarget)
	{
		return new OutfitBrowserScreenConfig(this.outfitType, sourceTarget, this.minionPersonality, this.targetMinionInstance);
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
		return new OutfitBrowserScreenConfig(Option.None, Option.None, Option.None, Option.None);
	}

	public static OutfitBrowserScreenConfig Minion(ClothingOutfitUtility.OutfitType outfitType, Personality personality)
	{
		return new OutfitBrowserScreenConfig(outfitType, Option.None, personality, Option.None);
	}

	public static OutfitBrowserScreenConfig Minion(ClothingOutfitUtility.OutfitType outfitType, GameObject minionInstance)
	{
		Personality personality = Db.Get().Personalities.Get(minionInstance.GetComponent<MinionIdentity>().personalityResourceId);
		return new OutfitBrowserScreenConfig(outfitType, ClothingOutfitTarget.FromMinion(outfitType, minionInstance), personality, minionInstance);
	}

	public static OutfitBrowserScreenConfig Minion(ClothingOutfitUtility.OutfitType outfitType, MinionBrowserScreen.GridItem item)
	{
		MinionBrowserScreen.GridItem.PersonalityTarget personalityTarget = item as MinionBrowserScreen.GridItem.PersonalityTarget;
		if (personalityTarget != null)
		{
			return OutfitBrowserScreenConfig.Minion(outfitType, personalityTarget.personality);
		}
		MinionBrowserScreen.GridItem.MinionInstanceTarget minionInstanceTarget = item as MinionBrowserScreen.GridItem.MinionInstanceTarget;
		if (minionInstanceTarget != null)
		{
			return OutfitBrowserScreenConfig.Minion(outfitType, minionInstanceTarget.minionInstance);
		}
		throw new NotImplementedException();
	}

	public void ApplyAndOpenScreen()
	{
		LockerNavigator.Instance.outfitBrowserScreen.GetComponent<OutfitBrowserScreen>().Configure(this);
		LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitBrowserScreen, null);
	}

	public readonly Option<ClothingOutfitUtility.OutfitType> outfitType;

	public readonly Option<ClothingOutfitTarget> selectedTarget;

	public readonly Option<Personality> minionPersonality;

	public readonly Option<GameObject> targetMinionInstance;

	public readonly bool isValid;

	public readonly bool isPickingOutfitForDupe;
}
