using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class ColdBrain : Sickness
	{
		public ColdBrain()
			: base("ColdSickness", Sickness.SicknessType.Ailment, Sickness.Severity.Minor, 0.005f, new List<Sickness.InfectionVector> { Sickness.InfectionVector.Inhalation }, 180f, null)
		{
			base.AddSicknessComponent(new CommonSickEffectSickness());
			base.AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME, false, false, true),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME, false, false, true),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME, false, false, true),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME, false, false, true),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME, false, false, true)
			}));
			base.AddSicknessComponent(new AnimatedSickness(new HashedString[] { "anim_idle_cold_kanim", "anim_loco_run_cold_kanim", "anim_loco_walk_cold_kanim" }, Db.Get().Expressions.SickCold));
			base.AddSicknessComponent(new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Cold, 15f));
		}

		public const string ID = "ColdSickness";
	}
}
