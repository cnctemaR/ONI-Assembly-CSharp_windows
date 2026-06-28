using System;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class ColdBrain : AnimatedAttributeModifierDisease
	{
		public ColdBrain()
			: base("ColdBrain", new HashedString[] { "anim_idle_cold_kanim", "anim_loco_run_cold_kanim", "anim_loco_walk_cold_kanim" }, "Cold", true, new AttributeModifier[]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false)
			}, 1f, 900f, new Disease.EffectProbabilityDelta[]
			{
				new Disease.EffectProbabilityDelta
				{
					effectID = "Hypothermia",
					probabilityDelta = 1f
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

		public override string InfectionSourceString()
		{
			return DUPLICANTS.DISEASES.INFECTIONSOURCES.INTERNAL_TEMPERATURE;
		}
	}
}
