using System;
using UnityEngine;

public class Sculpture : Artable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (Sculpture.sculptureOverrides == null)
		{
			Sculpture.sculptureOverrides = new KAnimFile[] { Assets.GetAnim("anim_interacts_sculpture_kanim") };
		}
		this.overrideAnims = Sculpture.sculptureOverrides;
		this.synchronizeAnims = false;
	}

	public override void SetStage(string stage_id, bool skip_effect)
	{
		base.SetStage(stage_id, skip_effect);
		bool flag = base.CurrentStage == "Default";
		if (Db.GetArtableStages().Get(stage_id) == null)
		{
			global::Debug.LogError("Missing stage: " + stage_id);
		}
		if (!skip_effect && !flag)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("sculpture_fx_kanim", base.transform.GetPosition(), base.transform, false, Grid.SceneLayer.Front, false);
			kbatchedAnimController.destroyOnAnimComplete = true;
			kbatchedAnimController.transform.SetLocalPosition(Vector3.zero);
			kbatchedAnimController.Play("poof", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private static KAnimFile[] sculptureOverrides;
}
