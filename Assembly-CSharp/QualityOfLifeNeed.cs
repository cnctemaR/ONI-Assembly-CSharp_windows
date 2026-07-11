using System;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class QualityOfLifeNeed : Need, ISim4000ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		this.expectationAttribute = attributes.Add(Db.Get().Attributes.QualityOfLifeExpectation);
		base.Name = DUPLICANTS.NEEDS.QUALITYOFLIFE.NAME;
		base.ExpectationTooltip = string.Format(DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_TOOLTIP, Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this).GetTotalValue());
		this.stressBonus = new Need.ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.NEEDS.QUALITYOFLIFE.GOOD_MODIFIER, false, false, false)
		};
		this.stressNeutral = new Need.ModifierType();
		this.stressPenalty = new Need.ModifierType
		{
			modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, DUPLICANTS.NEEDS.QUALITYOFLIFE.BAD_MODIFIER, false, false, false),
			statusItem = Db.Get().DuplicantStatusItems.PoorQualityOfLife
		};
		this.qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(base.gameObject);
	}

	public void Sim4000ms(float dt)
	{
		if (this.skipUpdate)
		{
			return;
		}
		float num = 1f;
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting("Morale");
		if (currentQualitySetting.id == "Disabled")
		{
			base.SetModifier(this.stressNeutral);
			return;
		}
		if (currentQualitySetting.id == "Easy")
		{
			num = 0.5f;
		}
		else if (currentQualitySetting.id == "Hard")
		{
			num = 1.5f;
		}
		else if (currentQualitySetting.id == "VeryHard")
		{
			num = 2f;
		}
		float totalValue = this.qolAttribute.GetTotalValue();
		float totalValue2 = this.expectationAttribute.GetTotalValue();
		float num2 = totalValue2 - totalValue;
		if (totalValue < totalValue2)
		{
			this.stressPenalty.modifier.SetValue(Mathf.Min(num2 * 0.008333334f, 0.041666668f) * num);
			base.SetModifier(this.stressPenalty);
		}
		else if (totalValue > totalValue2)
		{
			this.stressBonus.modifier.SetValue(Mathf.Max(-num2 * -0.016666668f, -0.041666668f) * num);
			base.SetModifier(this.stressBonus);
		}
		else
		{
			base.SetModifier(this.stressNeutral);
		}
	}

	private AttributeInstance qolAttribute;

	public bool skipUpdate;
}
