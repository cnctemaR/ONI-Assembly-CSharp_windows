using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class DecorNeed : Need
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, false, false);
		Attributes attributes = base.gameObject.GetAttributes();
		attributes.Add(Db.Get().Attributes.DecorExpectation);
		attributes.Add("Decor", this.modifier);
		this.amount = Db.Get().Amounts.Decor.Lookup(this);
		base.Name = DUPLICANTS.NEEDS.DECOR.NAME;
		base.ExpectationTooltip = DUPLICANTS.NEEDS.DECOR.EXPECTATION_TOOLTIP;
		this.expectationModifier = new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 0f, attributes.GetProfessionString(true), false, false);
		this.decorStressBonus = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.NEEDS.DECOR.NAME, false, false);
		this.decorStressNeutral = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.DECOR.NAME, false, false);
		this.decorStressPenalty = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, DUPLICANTS.NEEDS.DECOR.NAME, false, false);
		attributes.Add("Profession", this.expectationModifier);
		this.RefreshExpectations();
		this.Subscribe(-110704193, delegate(object data)
		{
			this.RefreshExpectations();
		});
	}

	private void RefreshExpectations()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		AttributeInstance profession = attributes.GetProfession();
		this.expectationModifier.Value = Math.Min(profession.GetTotalValue() * 5f, 75f);
		this.expectationAttribute = Db.Get().Attributes.DecorExpectation.Lookup(this);
	}

	public override Klei.AI.Attribute GetExpectationAttribute()
	{
		return Db.Get().Attributes.DecorExpectation;
	}

	private void Update()
	{
		if (this.skipUpdate)
		{
			return;
		}
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		float num2 = (float)GameUtil.GetDecorAtCell(num);
		float num3 = 0f;
		float num4 = 4.1666665f;
		if (Mathf.Abs(num2 - this.amount.value) > 0.1f)
		{
			if (num2 > this.amount.value)
			{
				num3 = 3f * num4;
			}
			else if (num2 < this.amount.value)
			{
				num3 = -num4;
			}
		}
		this.modifier.Value = num3;
		bool flag = false;
		float totalValue = this.expectationAttribute.GetTotalValue();
		AttributeModifier attributeModifier;
		if (this.amount.value <= 0f)
		{
			flag = true;
			attributeModifier = this.decorStressPenalty;
		}
		else if (this.amount.value >= totalValue)
		{
			attributeModifier = this.decorStressBonus;
		}
		else
		{
			attributeModifier = this.decorStressNeutral;
		}
		if (this.currentStressModifier != attributeModifier)
		{
			Attributes attributes = this.GetAttributes();
			if (this.currentStressModifier != null)
			{
				attributes.Remove(this.currentStressModifier);
			}
			if (attributeModifier != null)
			{
				attributes.Add(attributeModifier.Description, attributeModifier);
			}
			ThoughtGraph.Instance smi = this.GetSMI<ThoughtGraph.Instance>();
			if (smi != null)
			{
				if (flag)
				{
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.PoorDecor, this);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.PoorDecor, false);
				}
			}
			this.currentStressModifier = attributeModifier;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Attributes attributes = base.gameObject.GetAttributes();
		attributes.Remove(this.modifier);
	}

	private AttributeModifier modifier;

	private AmountInstance amount;

	private AttributeModifier expectationModifier;

	private AttributeModifier decorStressBonus;

	private AttributeModifier decorStressNeutral;

	private AttributeModifier decorStressPenalty;

	private AttributeModifier currentStressModifier;

	private AttributeInstance expectationAttribute;

	public bool skipUpdate;
}
