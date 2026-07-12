using System;
using UnityEngine;

public readonly struct OutfitDesignerScreenConfig
{
	public OutfitDesignerScreenConfig(Option<ClothingOutfitTarget> sourceTargetOpt, Option<Personality> minionPersonality, Option<GameObject> targetMinionInstance, Action<ClothingOutfitTarget> onWriteToOutfitTargetFn = null)
	{
		this.sourceTarget = (sourceTargetOpt.HasValue ? sourceTargetOpt.Value : ClothingOutfitTarget.ForNewOutfit());
		this.outfitTemplate = (this.sourceTarget.IsTemplateOutfit() ? Option.Some<ClothingOutfitTarget>(this.sourceTarget) : Option.None);
		this.minionPersonality = minionPersonality;
		this.targetMinionInstance = targetMinionInstance;
		this.onWriteToOutfitTargetFn = onWriteToOutfitTargetFn;
		this.isValid = true;
		ClothingOutfitTarget.MinionInstance minionInstance;
		if (this.sourceTarget.Is<ClothingOutfitTarget.MinionInstance>(out minionInstance))
		{
			global::Debug.Assert(targetMinionInstance.HasValue && targetMinionInstance == minionInstance.minionInstance);
		}
	}

	public OutfitDesignerScreenConfig WithOutfit(Option<ClothingOutfitTarget> sourceTarget)
	{
		return new OutfitDesignerScreenConfig(sourceTarget, this.minionPersonality, this.targetMinionInstance, this.onWriteToOutfitTargetFn);
	}

	public OutfitDesignerScreenConfig OnWriteToOutfitTarget(Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
	{
		return new OutfitDesignerScreenConfig(this.sourceTarget, this.minionPersonality, this.targetMinionInstance, onWriteToOutfitTargetFn);
	}

	public static OutfitDesignerScreenConfig Mannequin(Option<ClothingOutfitTarget> outfit)
	{
		return new OutfitDesignerScreenConfig(outfit, Option.None, Option.None, null);
	}

	public static OutfitDesignerScreenConfig Minion(Option<ClothingOutfitTarget> outfit, Personality personality)
	{
		return new OutfitDesignerScreenConfig(outfit, personality, Option.None, null);
	}

	public static OutfitDesignerScreenConfig Minion(Option<ClothingOutfitTarget> outfit, GameObject targetMinionInstance)
	{
		Personality personality = Db.Get().Personalities.Get(targetMinionInstance.GetComponent<MinionIdentity>().personalityResourceId);
		return new OutfitDesignerScreenConfig(outfit.HasValue ? outfit.Value : ClothingOutfitTarget.FromMinion(targetMinionInstance), personality, targetMinionInstance, null);
	}

	public void ApplyAndOpenScreen()
	{
		LockerNavigator.Instance.outfitDesignerScreen.GetComponent<OutfitDesignerScreen>().Configure(this);
		LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitDesignerScreen);
	}

	public readonly ClothingOutfitTarget sourceTarget;

	public readonly Option<ClothingOutfitTarget> outfitTemplate;

	public readonly Option<Personality> minionPersonality;

	public readonly Option<GameObject> targetMinionInstance;

	public readonly Action<ClothingOutfitTarget> onWriteToOutfitTargetFn;

	public readonly bool isValid;
}
