using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class ColdBrain : Disease
	{
		public ColdBrain()
			: base("ColdBrain", Disease.DiseaseType.Ailment, Disease.Severity.Minor, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 120f, 0, new Disease.RangeInfo(0f, 0f, 1000f, 1000f), new Disease.RangeInfo(1f, 1f, 1f, 1f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), new Disease.RangeInfo(1f, 1f, 1f, 1f))
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDBRAIN.NAME, false, false, true)
			}));
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_cold_kanim", "anim_loco_run_cold_kanim", "anim_loco_walk_cold_kanim" }, Db.Get().Expressions.SickCold));
			base.AddDiseaseComponent(new PeriodicEmoteDisease("anim_idle_cold_kanim", new HashedString[] { "idle_pre", "idle_default", "idle_pst" }, 5f));
		}

		public const string ID = "ColdBrain";
	}
}
