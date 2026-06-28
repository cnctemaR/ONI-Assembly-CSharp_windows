using System;
using Klei.AI;

public class RoleAttributePerk : RolePerk
{
	public RoleAttributePerk(string id, string description, string attributeId, float modifierBonus, string modifierDesc)
		: base(id, string.Format(description, modifierBonus), null, null, delegate(MinionResume identity)
		{
		}, false)
	{
		RoleAttributePerk $this = this;
		this.modifier = new AttributeModifier(attributeId, modifierBonus, modifierDesc, false, false, true);
		base.OnApply = delegate(MinionResume identity)
		{
			if (identity.GetAttributes().Get($this.modifier.AttributeId).Modifiers.Find((AttributeInstance.AttributeModifierEntry mod) => mod.Modifier == $this.modifier) == null)
			{
				identity.GetAttributes().Add("RolePerk_" + id, $this.modifier);
			}
		};
		base.OnRemove = delegate(MinionResume identity)
		{
			identity.GetAttributes().Remove($this.modifier);
		};
	}

	private AttributeModifier modifier;
}
