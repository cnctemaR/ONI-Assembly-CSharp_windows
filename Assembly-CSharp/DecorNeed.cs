using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class DecorNeed : Need, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.amount = Db.Get().Amounts.Decor.Lookup(base.gameObject);
		Attributes attributes = base.gameObject.GetAttributes();
		this.modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, false, false, false);
		this.expectationAttribute = attributes.Add(Db.Get().Attributes.DecorExpectation);
		attributes.Add("Decor", this.modifier);
		base.Name = DUPLICANTS.NEEDS.DECOR.NAME;
		base.ExpectationTooltip = string.Format(DUPLICANTS.NEEDS.DECOR.EXPECTATION_TOOLTIP, Db.Get().Attributes.DecorExpectation.Lookup(this).GetTotalValue());
		this.decorStressBonus = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.NEEDS.DECOR.NAME, false, false, true);
		this.decorStressNeutral = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.DECOR.NAME, false, false, true);
		this.decorStressPenalty = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, DUPLICANTS.NEEDS.DECOR.NAME, false, false, true);
		this.RefreshExpectations();
		base.Subscribe(-110704193, delegate(object data)
		{
			this.RefreshExpectations();
		});
	}

	private void RefreshExpectations()
	{
	}

	public override Klei.AI.Attribute GetExpectationAttribute()
	{
		return Db.Get().Attributes.DecorExpectation;
	}

	public void Sim200ms(float dt)
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
		float decorAtCell = GameUtil.GetDecorAtCell(num);
		float num2 = 0f;
		float num3 = 4.1666665f;
		if (Mathf.Abs(decorAtCell - this.amount.value) > 0.5f)
		{
			if (decorAtCell > this.amount.value)
			{
				num2 = 3f * num3;
			}
			else if (decorAtCell < this.amount.value)
			{
				num2 = -num3;
			}
		}
		else
		{
			this.amount.value = decorAtCell;
		}
		this.modifier.SetValue(num2);
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
				attributes.Add(attributeModifier.GetDescription(), attributeModifier);
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

	private AttributeModifier decorStressBonus;

	private AttributeModifier decorStressNeutral;

	private AttributeModifier decorStressPenalty;

	private AttributeModifier currentStressModifier;

	private AttributeInstance expectationAttribute;

	public bool skipUpdate;
}
