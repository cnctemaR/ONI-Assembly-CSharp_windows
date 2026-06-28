using System;
using UnityEngine;

namespace Klei.AI
{
	public class CommonSickEffectDisease : Disease.DiseaseComponent
	{
		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("contaminated_crew_fx_kanim", go.transform.GetPosition() + new Vector3(0f, 0f, -0.1f), go.transform, true, Grid.SceneLayer.Front, false);
			kbatchedAnimController.Play("fx_loop", KAnim.PlayMode.Loop, 1f, 0f);
			return kbatchedAnimController;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			KAnimControllerBase kanimControllerBase = (KAnimControllerBase)instance_data;
			kanimControllerBase.gameObject.DeleteObject();
		}
	}
}
