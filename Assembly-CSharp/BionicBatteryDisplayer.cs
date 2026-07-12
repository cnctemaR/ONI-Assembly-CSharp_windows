using System;
using Klei.AI;
using STRINGS;

public class BionicBatteryDisplayer : StandardAmountDisplayer
{
	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		BionicBatteryMonitor.Instance smi = instance.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
		string text = "";
		if (master.description.IndexOf("{1}") > -1)
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), GameUtil.GetIdentityDescriptor(instance.gameObject, this.tense));
		}
		else
		{
			text += string.Format(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
		}
		text += "\n\n";
		float num = instance.deltaAttribute.GetTotalDisplayValue();
		if (smi != null)
		{
			float wattage = smi.Wattage;
			num += wattage;
		}
		float num2 = ((num == 0f) ? 0f : (smi.CurrentCharge / num));
		text += string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.CURRENT_WATTAGE_TOOLTIP_LABEL, this.formatter.GetFormattedValue(num, this.formatter.DeltaTimeSlice));
		if (smi != null)
		{
			string text2 = "\n    • ";
			if (smi.IsBatterySaveModeActive)
			{
				string text3 = "<b>+</b>";
				text2 += string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.SAVING_MODE_TEMPLATE, DUPLICANTS.MODIFIERS.BIONIC_WATTS.BASE_NAME, DUPLICANTS.MODIFIERS.BIONIC_WATTS.SAVING_MODE_NAME, text3 + GameUtil.GetFormattedWattage(smi.GetBaseWattage(), GameUtil.WattageFormatterUnit.Automatic, true));
			}
			else
			{
				string text4 = "<b>+</b>";
				text2 += string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.STANDARD_ACTIVE_TEMPLATE, DUPLICANTS.MODIFIERS.BIONIC_WATTS.BASE_NAME, text4 + GameUtil.GetFormattedWattage(smi.GetBaseWattage(), GameUtil.WattageFormatterUnit.Automatic, true));
			}
			text += text2;
			float num3 = 0f;
			string text5 = "";
			foreach (BionicBatteryMonitor.WattageModifier wattageModifier in smi.Modifiers)
			{
				if (wattageModifier.value != 0f)
				{
					text = text + "\n    • " + wattageModifier.name;
				}
				else
				{
					text5 = text5 + "\n    • " + wattageModifier.name;
					num3 += wattageModifier.potentialValue;
				}
			}
			text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.POTENTIAL_EXTRA_WATTAGE_TOOLTIP_LABEL, this.formatter.GetFormattedValue(num3, this.formatter.DeltaTimeSlice)) + text5;
		}
		Debug.Assert(instance.deltaAttribute.Modifiers.Count <= 0, "Bionic Battery Displayer has found an invalid AttributeModifier. This particular Amount should not use AttributeModifiers, instead, use BionicBatteryMonitor.Instance.Modifiers");
		text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.BIONIC_WATTS.ESTIMATED_LIFE_TIME_REMAINING, GameUtil.GetFormattedCycles(num2, "F1", false));
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

	public class BionicBatteryAttributeFormatter : StandardAttributeFormatter
	{
		public BionicBatteryAttributeFormatter()
			: base(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond)
		{
		}
	}
}
