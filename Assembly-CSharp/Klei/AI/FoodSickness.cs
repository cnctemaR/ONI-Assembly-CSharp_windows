using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class FoodSickness : Sickness
	{
		public FoodSickness()
			: base("FoodSickness", Sickness.SicknessType.Pathogen, Sickness.Severity.Minor, 0.005f, new List<Sickness.InfectionVector> { Sickness.InfectionVector.Digestion }, 1020f)
		{
			base.AddSicknessComponent(new CommonSickEffectSickness());
			base.AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[]
			{
				new AttributeModifier("BladderDelta", 0.33333334f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME, false, false, true),
				new AttributeModifier("ToiletEfficiency", -0.2f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME, false, false, true),
				new AttributeModifier("StaminaDelta", -0.05f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME, false, false, true)
			}));
			base.AddSicknessComponent(new AnimatedSickness(new HashedString[] { "anim_idle_sick_kanim" }, Db.Get().Expressions.Sick));
			base.AddSicknessComponent(new PeriodicEmoteSickness("anim_idle_sick_kanim", new HashedString[] { "idle_pre", "idle_default", "idle_pst" }, 5f));
		}

		public const string ID = "FoodSickness";

		private const float VOMIT_FREQUENCY = 200f;
	}
}
