using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class Allergies : Sickness
	{
		public Allergies()
			: base("Allergies", Sickness.SicknessType.Pathogen, Sickness.Severity.Minor, 0.00025f, new List<Sickness.InfectionVector> { Sickness.InfectionVector.Inhalation }, 60f, null)
		{
			float num = 0.025f;
			base.AddSicknessComponent(new CommonSickEffectSickness());
			base.AddSicknessComponent(new AnimatedSickness(new HashedString[] { "anim_idle_allergies_kanim" }, Db.Get().Expressions.Pollen));
			base.AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, num, DUPLICANTS.DISEASES.ALLERGIES.NAME, false, false, true),
				new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 10f, DUPLICANTS.DISEASES.ALLERGIES.NAME, false, false, true)
			}));
		}

		public const string ID = "Allergies";

		public const float STRESS_PER_CYCLE = 15f;
	}
}
