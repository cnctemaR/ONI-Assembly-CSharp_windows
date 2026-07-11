using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class Sunburn : Disease
	{
		public Sunburn()
			: base("Sunburn", Disease.DiseaseType.Ailment, Disease.Severity.Major, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Exposure }, 900f, 0, new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, DUPLICANTS.DISEASES.SUNBURN.NAME, false, false, true)
			}));
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_hot_kanim" }, "Hot"));
		}

		public const string ID = "Sunburn";
	}
}
