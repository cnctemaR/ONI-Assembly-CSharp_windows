using System;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class Diarrhea : AttributeModifierDisease
	{
		public Diarrhea()
			: base("Diarrhea", false, new AttributeModifier[]
			{
				new AttributeModifier("BladderDelta", 1.25f, DUPLICANTS.DISEASES.DIARRHEA.NAME, false),
				new AttributeModifier("ToiletEfficiency", -0.8f, DUPLICANTS.DISEASES.DIARRHEA.NAME, true)
			}, 0.005f, 900f, new Disease.EffectProbabilityDelta[]
			{
				new Disease.EffectProbabilityDelta
				{
					effectID = "DirtyHands",
					probabilityDelta = 0.005f
				}
			})
		{
		}

		protected override object OnInfect(GameObject go)
		{
			base.OnInfect(go);
			return base.StartCommonSickEffect(go);
		}

		protected override void OnCure(GameObject go, object instance_data)
		{
			base.OnCure(go, instance_data);
			KAnimControllerBase kanimControllerBase = (KAnimControllerBase)instance_data;
			kanimControllerBase.gameObject.DeleteObject();
		}
	}
}
