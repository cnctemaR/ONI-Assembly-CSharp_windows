using System;
using Klei.AI;

public abstract class Need : KMonoBehaviour
{
	public string Name { get; protected set; }

	public string ExpectationTooltip { get; protected set; }

	public string Tooltip { get; protected set; }

	public Klei.AI.Attribute GetExpectationAttribute()
	{
		return this.expectationAttribute.Attribute;
	}

	protected void SetModifier(Need.ModifierType modifier)
	{
		if (this.currentStressModifier != modifier)
		{
			if (this.currentStressModifier != null)
			{
				this.UnapplyModifier(this.currentStressModifier);
			}
			if (modifier != null)
			{
				this.ApplyModifier(modifier);
			}
			this.currentStressModifier = modifier;
		}
	}

	private void ApplyModifier(Need.ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			this.GetAttributes().Add(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			base.GetComponent<KSelectable>().AddStatusItem(modifier.statusItem, null);
		}
		if (modifier.thought != null)
		{
			this.GetSMI<ThoughtGraph.Instance>().AddThought(modifier.thought);
		}
	}

	private void UnapplyModifier(Need.ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			this.GetAttributes().Remove(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(modifier.statusItem, false);
		}
		if (modifier.thought != null)
		{
			this.GetSMI<ThoughtGraph.Instance>().RemoveThought(modifier.thought);
		}
	}

	protected AttributeInstance expectationAttribute;

	protected Need.ModifierType stressBonus;

	protected Need.ModifierType stressNeutral;

	protected Need.ModifierType stressPenalty;

	protected Need.ModifierType currentStressModifier;

	protected class ModifierType
	{
		public AttributeModifier modifier;

		public StatusItem statusItem;

		public Thought thought;
	}
}
