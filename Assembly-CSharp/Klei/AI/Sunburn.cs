using System;
using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class Sunburn : Sickness
	{
		public Sunburn()
			: base("SunburnSickness", Sickness.SicknessType.Ailment, Sickness.Severity.Minor, 0.005f, new List<Sickness.InfectionVector> { Sickness.InfectionVector.Exposure }, 1020f, null)
		{
			base.AddSicknessComponent(new CommonSickEffectSickness());
			base.AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[]
			{
				new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, DUPLICANTS.DISEASES.SUNBURNSICKNESS.NAME, false, false, true)
			}));
			base.AddSicknessComponent(new AnimatedSickness(new HashedString[] { "anim_idle_hot_kanim", "anim_loco_run_hot_kanim", "anim_loco_walk_hot_kanim" }, Db.Get().Expressions.SickFierySkin));
			base.AddSicknessComponent(new PeriodicEmoteSickness(Db.Get().Emotes.Minion.Hot, 5f));
		}

		public const string ID = "SunburnSickness";
	}
}
