using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class LungCapacityPerk : SkillPerk
	{
		public LungCapacityPerk(string id, float modifierBonus, string modifierDesc)
			: base(id, "", null, null, null, null, false)
		{
			this.bonus = modifierBonus;
			string id2 = Db.Get().Amounts.Breath.maxAttribute.Id;
			Klei.AI.Attribute attribute = Db.Get().Attributes.Get(id2);
			this.modifier = new AttributeModifier(id2, this.bonus, modifierDesc, false, false, true);
			this.Name = string.Format(UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, this.modifier.GetFormattedString(), attribute.Name);
			base.OnApply = new Action<MinionResume>(this.ApplyPerk);
			base.OnRemove = new Action<MinionResume>(this.RemovePerk);
		}

		private void ApplyPerk(MinionResume identity)
		{
			ArrayRef<AttributeModifier> modifiers = identity.GetAttributes().Get(this.modifier.AttributeId).Modifiers;
			bool flag = false;
			for (int num = 0; num != modifiers.Count; num++)
			{
				if (modifiers[num] == this.modifier)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				identity.GetAttributes().Add(this.modifier);
			}
			if (this.breathBoostConsumed.Add(identity))
			{
				AmountInstance amountInstance = Db.Get().Amounts.Breath.Lookup(identity);
				if (amountInstance != null)
				{
					amountInstance.SetValue(amountInstance.value + this.bonus);
				}
			}
		}

		private void RemovePerk(MinionResume identity)
		{
			identity.GetAttributes().Remove(this.modifier);
			this.breathBoostConsumed.Add(identity);
		}

		private HashSet<MinionResume> breathBoostConsumed = new HashSet<MinionResume>();

		private AttributeModifier modifier;

		private float bonus;
	}
}
