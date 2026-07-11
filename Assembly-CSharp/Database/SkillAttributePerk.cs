using System;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class SkillAttributePerk : SkillPerk
	{
		public SkillAttributePerk(string id, string attributeId, float modifierBonus, string modifierDesc)
			: base(id, string.Empty, null, null, delegate(MinionResume identity)
			{
			}, false)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attributeId);
			this.modifier = new AttributeModifier(attributeId, modifierBonus, modifierDesc, false, false, true);
			this.Name = string.Format(UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, this.modifier.GetFormattedString(null), attribute.Name);
			base.OnApply = delegate(MinionResume identity)
			{
				if (identity.GetAttributes().Get(this.modifier.AttributeId).Modifiers.FindIndex((AttributeModifier mod) => mod == this.modifier) == -1)
				{
					identity.GetAttributes().Add(this.modifier);
				}
			};
			base.OnRemove = delegate(MinionResume identity)
			{
				identity.GetAttributes().Remove(this.modifier);
			};
		}

		public AttributeModifier modifier;
	}
}
