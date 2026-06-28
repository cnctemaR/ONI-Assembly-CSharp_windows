using System;
using UnityEngine;

public class Sculpture : Artable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (Sculpture.sculptureOverrides == null)
		{
			Sculpture.sculptureOverrides = new KAnimFile[] { Assets.GetAnim("anim_interacts_sculpture") };
		}
		this.overrideAnims = Sculpture.sculptureOverrides;
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return Sculpture.WorkAnims;
	}

	public override void SetStage(string stage_id, bool skip_effect)
	{
		base.SetStage(stage_id, skip_effect);
		if (!skip_effect && base.CurrentStage != "Default")
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("sculpture_fx", this.transform, false, Grid.SceneLayer.Front);
			kbatchedAnimController.destroyOnAnimComplete = true;
			kbatchedAnimController.transform.localPosition = Vector3.zero;
			kbatchedAnimController.Play("poof", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private static readonly string[] WorkAnims = new string[] { "working_pre", "working_loop" };

	private static KAnimFile[] sculptureOverrides = null;
}
