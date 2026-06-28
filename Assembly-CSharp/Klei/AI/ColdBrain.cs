using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class ColdBrain : Disease
	{
		public ColdBrain()
			: base("ColdBrain", Disease.DiseaseType.Ailment, Disease.Severity.Major, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 900f, 0, new Disease.RangeInfo(0f, 0f, 1000f, 1000f), new Disease.RangeInfo(1f, 1f, 1f, 1f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), new Disease.RangeInfo(1f, 1f, 1f, 1f))
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false)
			}));
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_cold_kanim", "anim_loco_run_cold_kanim", "anim_loco_walk_cold_kanim" }, "Cold"));
		}

		public const string ID = "ColdBrain";
	}
}
