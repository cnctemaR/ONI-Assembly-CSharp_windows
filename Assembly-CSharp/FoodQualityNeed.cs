using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class FoodQualityNeed : Need
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		attributes.Add(Db.Get().Attributes.FoodExpectation);
		base.Name = DUPLICANTS.NEEDS.FOOD_QUALITY.NAME;
		this.expectationModifier = new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 0f, attributes.GetProfessionString(true), false, false);
		this.foodQualitStressBonus = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.NEEDS.FOOD_QUALITY.GOOD_FOOD_MOD, false, false);
		this.foodQualityStressNeutral = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0f, DUPLICANTS.NEEDS.FOOD_QUALITY.NORMAL_FOOD_MOD, false, false);
		this.foodQualityStressPenalty = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, DUPLICANTS.NEEDS.FOOD_QUALITY.BAD_FOOD_MOD, false, false);
		attributes.Add("Profession", this.expectationModifier);
		this.RefreshExpectations();
		base.ExpectationTooltip = string.Format(DUPLICANTS.NEEDS.FOOD_QUALITY.EXPECTATION_TOOLTIP, Db.Get().Attributes.FoodExpectation.Lookup(this).GetTotalValue());
		this.Subscribe(-110704193, delegate(object data)
		{
			this.RefreshExpectations();
		});
		this.Subscribe(1406130139, new Action<object>(this.OnEatStart));
		this.Subscribe(1121894420, new Action<object>(this.OnEatComplete));
	}

	private void RefreshExpectations()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		AttributeInstance profession = attributes.GetProfession();
		this.expectationModifier.Value = Math.Min(Mathf.Floor(profession.GetTotalValue() / 5f), 3f);
		this.expectationAttribute = Db.Get().Attributes.FoodExpectation.Lookup(this);
	}

	public override Klei.AI.Attribute GetExpectationAttribute()
	{
		return Db.Get().Attributes.FoodExpectation;
	}

	private void OnEatComplete(object data)
	{
		Attributes attributes = this.GetAttributes();
		if (this.currentStressModifier != null)
		{
			attributes.Remove(this.currentStressModifier);
			this.currentStressModifier = null;
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.PoorFoodQuality, false);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.GoodFoodQuality, false);
		ThoughtGraph.Instance smi = this.GetSMI<ThoughtGraph.Instance>();
		if (smi != null)
		{
			smi.RemoveThought(Db.Get().Thoughts.PoorFoodQuality);
			smi.RemoveThought(Db.Get().Thoughts.GoodFoodQuality);
		}
	}

	private void OnEatStart(object data)
	{
		Edible edible = (Edible)data;
		int quality = edible.FoodInfo.Quality;
		bool flag = false;
		bool flag2 = false;
		float totalValue = this.expectationAttribute.GetTotalValue();
		AttributeModifier attributeModifier;
		if ((float)quality < totalValue - 1f)
		{
			flag = true;
			attributeModifier = this.foodQualityStressPenalty;
		}
		else if ((float)quality > totalValue + 1f)
		{
			flag2 = true;
			attributeModifier = this.foodQualitStressBonus;
		}
		else
		{
			attributeModifier = this.foodQualityStressNeutral;
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
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.PoorFoodQuality, this);
					smi.AddThought(Db.Get().Thoughts.PoorFoodQuality);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.PoorFoodQuality, false);
					smi.RemoveThought(Db.Get().Thoughts.PoorFoodQuality);
				}
				if (flag2)
				{
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.GoodFoodQuality, this);
					smi.AddThought(Db.Get().Thoughts.GoodFoodQuality);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.GoodFoodQuality, false);
					smi.RemoveThought(Db.Get().Thoughts.GoodFoodQuality);
				}
			}
			this.currentStressModifier = attributeModifier;
		}
	}

	private AttributeModifier expectationModifier;

	private AttributeModifier foodQualitStressBonus;

	private AttributeModifier foodQualityStressNeutral;

	private AttributeModifier foodQualityStressPenalty;

	private AttributeModifier currentStressModifier;

	private AttributeInstance expectationAttribute;

	public bool skipUpdate;
}
