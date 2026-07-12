using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class UIMinion : KMonoBehaviour, UIMinionOrMannequin.ITarget
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

	public Option<Personality> Personality { get; private set; }

	protected override void OnSpawn()
	{
		this.TrySpawn();
	}

	public void TrySpawn()
	{
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(Assets.GetPrefab(MinionUIPortrait.ID), base.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.gameObject.SetActive(true);
			this.animController.animScale = 0.38f;
			this.animController.Play("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
			this.spawn = this.animController.gameObject;
		}
	}

	public void SetMinion(Personality personality)
	{
		this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyMinionPersonality(personality);
		this.Personality = personality;
		CustomOutfit customOutfit = this.SpawnedAvatar.AddOrGet<CustomOutfit>();
		customOutfit.enabled = false;
		if (personality.Name == "Jorge")
		{
			customOutfit.Update();
		}
	}

	public void SetOutfit(IEnumerable<ClothingItemResource> outfit)
	{
		this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyClothingItems(outfit, true);
	}

	public void React(UIMinionOrMannequinReactSource source)
	{
		if (source != UIMinionOrMannequinReactSource.OnPersonalityChanged && this.lastReactSource == source)
		{
			KAnim.Anim currentAnim = this.animController.GetCurrentAnim();
			if (currentAnim != null && currentAnim.name != "idle_default")
			{
				return;
			}
		}
		switch (source)
		{
		case UIMinionOrMannequinReactSource.OnPersonalityChanged:
			this.animController.Play("react", KAnim.PlayMode.Once, 1f, 0f);
			goto IL_016C;
		case UIMinionOrMannequinReactSource.OnWholeOutfitChanged:
		case UIMinionOrMannequinReactSource.OnBottomChanged:
			this.animController.Play("react_bottoms", KAnim.PlayMode.Once, 1f, 0f);
			goto IL_016C;
		case UIMinionOrMannequinReactSource.OnTopChanged:
			this.animController.Play("react_tops", KAnim.PlayMode.Once, 1f, 0f);
			goto IL_016C;
		case UIMinionOrMannequinReactSource.OnGlovesChanged:
			this.animController.Play("react_gloves", KAnim.PlayMode.Once, 1f, 0f);
			goto IL_016C;
		case UIMinionOrMannequinReactSource.OnShoesChanged:
			this.animController.Play("react_shoes", KAnim.PlayMode.Once, 1f, 0f);
			goto IL_016C;
		}
		this.animController.Play("cheer_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Queue("cheer_loop", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Queue("cheer_pst", KAnim.PlayMode.Once, 1f, 0f);
		IL_016C:
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		this.lastReactSource = source;
	}

	public const float ANIM_SCALE = 0.38f;

	private KBatchedAnimController animController;

	private GameObject spawn;

	private UIMinionOrMannequinReactSource lastReactSource;
}
