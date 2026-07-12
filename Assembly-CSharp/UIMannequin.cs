using System;
using System.Collections.Generic;
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
		if (this.shouldShowOutfitWithDefaultItems)
		{
			outfit = UIMinionOrMannequinITargetExtensions.GetOutfitWithDefaultItems(outfitType, outfit);
		}
		MinionConfig.ConfigureSymbols(this.SpawnedAvatar, false);
		SymbolOverrideController component = this.SpawnedAvatar.GetComponent<SymbolOverrideController>();
		foreach (ClothingItemResource clothingItemResource in outfit)
		{
			KAnim.Build build = clothingItemResource.AnimFile.GetData().build;
			if (build != null)
			{
				for (int i = 0; i < build.symbols.Length; i++)
				{
					string text = HashCache.Get().Get(build.symbols[i].hash);
					component.AddSymbolOverride(text, build.symbols[i], 0);
					this.animController.SetSymbolVisiblity(text, true);
				}
			}
		}
	}

	public void React(UIMinionOrMannequinReactSource source)
	{
		this.animController.Play("idle", KAnim.PlayMode.Once, 1f, 0f);
	}

	public const float ANIM_SCALE = 0.38f;

	private KBatchedAnimController animController;

	private GameObject spawn;

	public bool shouldShowOutfitWithDefaultItems = true;
}
