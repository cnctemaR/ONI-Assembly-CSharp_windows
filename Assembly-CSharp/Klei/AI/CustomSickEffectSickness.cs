using System;
using UnityEngine;

namespace Klei.AI
{
	public class CustomSickEffectSickness : Sickness.SicknessComponent
	{
		public CustomSickEffectSickness(string effect_kanim, string effect_anim_name)
		{
			this.kanim = effect_kanim;
			this.animName = effect_anim_name;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect(this.kanim, go.transform.GetPosition() + new Vector3(0f, 0f, -0.1f), go.transform, true, Grid.SceneLayer.Front, false);
			kbatchedAnimController.Play(this.animName, KAnim.PlayMode.Loop, 1f, 0f);
			return kbatchedAnimController;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			((KAnimControllerBase)instance_data).gameObject.DeleteObject();
		}

		private string kanim;

		private string animName;
	}
}
