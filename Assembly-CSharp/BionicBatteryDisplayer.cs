using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicBatteryDisplayer : StandardAmountDisplayer
{
	private string GetIconForState(BionicBatteryDisplayer.ElectrobankState state)
	{
		switch (state)
		{
		case BionicBatteryDisplayer.ElectrobankState.Unexistent:
			return BionicBatteryMonitor.EmptySlotBatteryIcon;
		case BionicBatteryDisplayer.ElectrobankState.Charged:
			return BionicBatteryMonitor.ChargedBatteryIcon;
		}
		return BionicBatteryMonitor.DischargedBatteryIcon;
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		BionicBatteryMonitor.Instance smi = instance.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
		string text = "";
		float num = instance.deltaAttribute.GetTotalDisplayValue();
		if (smi != null)
		{
			float wattage = smi.Wattage;
			num += wattage;
		}
		if (master.description.IndexOf("{1}") > -1)
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), GameUtil.GetIdentityDescriptor(instance.gameObject, this.tense));
		}
		else
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
		}
		if (smi != null)
		{
			int electrobankCount = smi.ElectrobankCount;
			int electrobankCountCapacity = smi.ElectrobankCountCapacity;
			text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_DETAILS_LABEL, GameUtil.GetFormattedInt((float)electrobankCount, GameUtil.TimeSlice.None), GameUtil.GetFormattedInt((float)electrobankCountCapacity, GameUtil.TimeSlice.None));
			string text2 = "\n    • ";
			if (electrobankCount > 0)
			{
				for (int i = 0; i < smi.storage.items.Count; i++)
				{
					GameObject gameObject = smi.storage.items[i];
					Electrobank component = gameObject.GetComponent<Electrobank>();
					BionicBatteryDisplayer.ElectrobankState electrobankState = ((component == null) ? BionicBatteryDisplayer.ElectrobankState.Damaged : ((component.Charge <= 0f) ? BionicBatteryDisplayer.ElectrobankState.Depleated : BionicBatteryDisplayer.ElectrobankState.Charged));
					string iconForState = this.GetIconForState(electrobankState);
					float num2 = ((component == null) ? 0f : component.Charge);
					text = text + text2 + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_ROW, iconForState, gameObject.GetProperName(), GameUtil.GetFormattedJoules(num2, "F1", GameUtil.TimeSlice.None));
				}
			}
			if (electrobankCount < electrobankCountCapacity)
			{
				for (int j = 0; j < electrobankCountCapacity - electrobankCount; j++)
				{
					text = text + text2 + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_EMPTY_ROW, this.GetIconForState(BionicBatteryDisplayer.ElectrobankState.Unexistent));
				}
			}
		}
		text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.CURRENT_WATTAGE_LABEL, this.formatter.GetFormattedValue(num, this.formatter.DeltaTimeSlice));
		if (smi != null)
		{
			string text3 = "\n    • ";
			string text4 = "<b>+</b>";
			text3 += string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, DUPLICANTS.MODIFIERS.BIONIC_WATTS.BASE_NAME, text4 + GameUtil.GetFormattedWattage(smi.GetBaseWattage(), GameUtil.WattageFormatterUnit.Automatic, true));
			text += text3;
			float num3 = 0f;
			string text5 = "";
			foreach (BionicBatteryMonitor.WattageModifier wattageModifier in smi.Modifiers)
			{
				if (wattageModifier.value != 0f)
				{
					text = text + "\n    • " + wattageModifier.name;
				}
				else if (wattageModifier.potentialValue > 0f)
				{
					text5 = text5 + "\n    • " + wattageModifier.name;
					num3 += wattageModifier.potentialValue;
				}
			}
			if (!string.IsNullOrEmpty(text5))
			{
				text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.POTENTIAL_EXTRA_WATTAGE_LABEL, this.formatter.GetFormattedValue(num3, this.formatter.DeltaTimeSlice)) + text5;
			}
		}
		global::Debug.Assert(instance.deltaAttribute.Modifiers.Count <= 0, "Bionic Battery Displayer has found an invalid AttributeModifier. This particular Amount should not use AttributeModifiers, instead, use BionicBatteryMonitor.Instance.Modifiers");
		float num4 = ((num == 0f) ? 0f : (smi.CurrentCharge / num));
		text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ESTIMATED_LIFE_TIME_REMAINING, GameUtil.GetFormattedCycles(num4, "F1", false));
		return text;
	}

	public override string GetValueString(Amount master, AmountInstance instance)
	{
		return base.GetValueString(master, instance);
	}

	public BionicBatteryDisplayer()
		: base(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal)
	{
		this.formatter = new BionicBatteryDisplayer.BionicBatteryAttributeFormatter();
	}

	private const float criticalIconFlashFrequency = 0.45f;

	private enum ElectrobankState
	{
		Unexistent,
		Damaged,
		Depleated,
		Charged
	}

	public class BionicBatteryAttributeFormatter : StandardAttributeFormatter
	{
		public BionicBatteryAttributeFormatter()
			: base(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond)
		{
		}
	}
}
