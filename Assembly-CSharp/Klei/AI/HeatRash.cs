using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class HeatRash : Disease
	{
		public HeatRash()
			: base("HeatRash", Disease.DiseaseType.Ailment, Disease.Severity.Major, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 900f, 0, new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.HEATRASH.NAME, false, false)
			}));
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_hot_kanim" }, "Hot"));
		}

		public const string ID = "HeatRash";
	}
}
