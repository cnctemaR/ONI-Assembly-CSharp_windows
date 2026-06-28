using System;
using UnityEngine;

namespace Klei.AI
{
	public class DecorEntitlement : Entitlement
	{
		public DecorEntitlement(float modifierAmount)
		{
			this.modifier = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, modifierAmount, "<dev> from role", false, false, true);
		}

		public override void Apply(GameObject target)
		{
			Attributes attributes = target.GetAttributes();
			attributes.Add("role entitlement", this.modifier);
		}

		public override void Unapply(GameObject target)
		{
			Attributes attributes = target.GetAttributes();
			attributes.Remove(this.modifier);
		}

		private AttributeModifier modifier;
	}
}
