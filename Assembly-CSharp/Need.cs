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
			Attributes attributes = this.GetAttributes();
			attributes.Add(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			component.AddStatusItem(modifier.statusItem, null);
		}
		if (modifier.thought != null)
		{
			ThoughtGraph.Instance smi = this.GetSMI<ThoughtGraph.Instance>();
			smi.AddThought(modifier.thought);
		}
	}

	private void UnapplyModifier(Need.ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			Attributes attributes = this.GetAttributes();
			attributes.Remove(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			component.RemoveStatusItem(modifier.statusItem, false);
		}
		if (modifier.thought != null)
		{
			ThoughtGraph.Instance smi = this.GetSMI<ThoughtGraph.Instance>();
			smi.RemoveThought(modifier.thought);
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
