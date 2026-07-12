using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;

public class UIMannequin : KMonoBehaviour, UIMinionOrMannequin.ITarget
{
	public GameObject SpawnedAvatar
	{
		get
		{
			if (this.spawn == null)
			{
				this.TrySpawn();
			}
			return this.spawn;
		}
	}

	public Option<Personality> Personality
	{
		get
		{
			return default(Option<Personality>);
		}
	}

	protected override void OnSpawn()
	{
		this.TrySpawn();
	}

	public void TrySpawn()
	{
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(Assets.GetPrefab(MannequinUIPortrait.ID), base.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.LoadAnims();
			this.animController.gameObject.SetActive(true);
			this.animController.animScale = 0.38f;
			this.animController.Play("idle", KAnim.PlayMode.Paused, 1f, 0f);
			this.spawn = this.animController.gameObject;
			MinionConfig.ConfigureSymbols(this.spawn, false);
			base.gameObject.AddOrGet<MinionVoiceProviderMB>().voice = Option.None;
		}
	}

	public void SetOutfit(ClothingOutfitUtility.OutfitType outfitType, IEnumerable<ClothingItemResource> outfit)
	{
		bool flag = outfit.Count<ClothingItemResource>() == 0;
		if (this.shouldShowOutfitWithDefaultItems)
		{
			outfit = UIMinionOrMannequinITargetExtensions.GetOutfitWithDefaultItems(outfitType, outfit);
		}
		MinionConfig.ConfigureSymbols(this.SpawnedAvatar, false);
		SymbolOverrideController component = this.SpawnedAvatar.GetComponent<SymbolOverrideController>();
		Accessorizer component2 = this.SpawnedAvatar.GetComponent<Accessorizer>();
		WearableAccessorizer component3 = this.SpawnedAvatar.GetComponent<WearableAccessorizer>();
		component.RemoveAllSymbolOverrides(0);
		if (this.shouldShowOutfitWithDefaultItems && outfitType == ClothingOutfitUtility.OutfitType.Clothing)
		{
			component2.ApplyMinionPersonality(this.personalityToUseForDefaultClothing.UnwrapOr(Db.Get().Personalities.Get("ABE"), null));
			component3.UpdateVisibleSymbols(outfitType);
			foreach (string text in UIMannequin.DEFAULT_CLOTHING_SYMBOLS_TO_SHOW_AND_HIDE)
			{
				this.animController.SetSymbolVisiblity(text, true);
			}
			if (!flag)
			{
				this.animController.SetSymbolVisiblity("belt", false);
			}
			ClothingItemResource itemForCategory = UIMannequin.GetItemForCategory(PermitCategory.DupeGloves, outfit);
			ClothingItemResource itemForCategory2 = UIMannequin.GetItemForCategory(PermitCategory.DupeTops, outfit);
			if (itemForCategory != null && itemForCategory2 != null && itemForCategory.AnimFile.GetData().build.GetSymbol("arm_lower_sleeve") == null && itemForCategory2.AnimFile.GetData().build.GetSymbol("arm_lower_sleeve") == null)
			{
				this.animController.SetSymbolVisiblity("arm_lower_sleeve", false);
			}
		}
		else
		{
			foreach (string text2 in UIMannequin.DEFAULT_CLOTHING_SYMBOLS_TO_SHOW_AND_HIDE)
			{
				this.animController.SetSymbolVisiblity(text2, false);
			}
		}
		foreach (ClothingItemResource clothingItemResource in outfit)
		{
			KAnim.Build build = clothingItemResource.AnimFile.GetData().build;
			if (build != null)
			{
				for (int j = 0; j < build.symbols.Length; j++)
				{
					string text3 = HashCache.Get().Get(build.symbols[j].hash);
					component.AddSymbolOverride(text3, build.symbols[j], 0);
					this.animController.SetSymbolVisiblity(text3, true);
				}
			}
		}
	}

	private static ClothingItemResource GetItemForCategory(PermitCategory category, IEnumerable<ClothingItemResource> outfit)
	{
		foreach (ClothingItemResource clothingItemResource in outfit)
		{
			if (clothingItemResource.Category == category)
			{
				return clothingItemResource;
			}
		}
		return null;
	}

	public void React(UIMinionOrMannequinReactSource source)
	{
		this.animController.Play("idle", KAnim.PlayMode.Once, 1f, 0f);
	}

	public const float ANIM_SCALE = 0.38f;

	private KBatchedAnimController animController;

	private GameObject spawn;

	public bool shouldShowOutfitWithDefaultItems = true;

	public Option<Personality> personalityToUseForDefaultClothing;

	private static readonly string[] DEFAULT_CLOTHING_SYMBOLS_TO_SHOW_AND_HIDE = new string[]
	{
		"arm_lower", "arm_lower_sleeve", "arm_sleeve", "belt", "cuff", "foot", "hand_paint", "leg", "neck", "pelvis",
		"torso"
	};
}
