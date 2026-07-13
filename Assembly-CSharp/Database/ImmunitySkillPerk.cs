using System;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class ImmunitySkillPerk : SkillPerk
	{
		public ImmunitySkillPerk(string id, string nameOfEffectToBecomeImmuneTo)
			: base(id, "", null, null, delegate(MinionResume identity)
			{
			}, null, false)
		{
			Effect effect = Db.Get().effects.Get(nameOfEffectToBecomeImmuneTo);
			this.Name = GameUtil.SafeStringFormat(UI.ROLES_SCREEN.PERKS.IMMUNITY, new object[] { effect.Name });
			base.OnApply = delegate(MinionResume identity)
			{
				Effects component = identity.GetComponent<Effects>();
				if (component != null)
				{
					component.AddImmunity(effect, id, false);
				}
			};
			base.OnRemove = delegate(MinionResume identity)
			{
				Effects component2 = identity.GetComponent<Effects>();
				if (component2 != null)
				{
					component2.RemoveImmunity(effect, id);
				}
			};
		}
	}
}
