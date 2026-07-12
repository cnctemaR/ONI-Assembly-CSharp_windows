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
			this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("hand_paint", false);
			this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("foot", false);
			this.SpawnedAvatar.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("torso", false);
		}
	}

	public void SetOutfit(IEnumerable<ClothingItemResource> outfit)
	{
		this.SpawnedAvatar.GetComponent<Accessorizer>().ApplyClothingItems(outfit, false);
	}

	public void React(UIMinionOrMannequinReactSource source)
	{
		this.animController.Play("idle", KAnim.PlayMode.Once, 1f, 0f);
	}

	public const float ANIM_SCALE = 0.38f;

	private KBatchedAnimController animController;

	private GameObject spawn;
}
