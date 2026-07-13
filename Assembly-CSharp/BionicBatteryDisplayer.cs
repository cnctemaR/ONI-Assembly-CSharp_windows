using System;
using System.Text;
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
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		BionicBatteryMonitor.Instance smi = instance.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
		float num = instance.deltaAttribute.GetTotalDisplayValue();
		if (smi != null)
		{
			float wattage = smi.Wattage;
			num += wattage;
		}
		if (master.description.IndexOf("{1}") > -1)
		{
			stringBuilder.AppendFormat(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None), GameUtil.GetIdentityDescriptor(instance.gameObject, this.tense));
		}
		else
		{
			stringBuilder.AppendFormat(master.description, this.formatter.GetFormattedValue(instance.value, GameUtil.TimeSlice.None));
		}
		if (smi != null)
		{
			int electrobankCount = smi.ElectrobankCount;
			int electrobankCountCapacity = smi.ElectrobankCountCapacity;
			stringBuilder.Append("\n\n");
			stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_DETAILS_LABEL, GameUtil.GetFormattedInt((float)electrobankCount, GameUtil.TimeSlice.None), GameUtil.GetFormattedInt((float)electrobankCountCapacity, GameUtil.TimeSlice.None));
			if (electrobankCount > 0)
			{
				for (int i = 0; i < smi.storage.items.Count; i++)
				{
					GameObject gameObject = smi.storage.items[i];
					Electrobank component = gameObject.GetComponent<Electrobank>();
					BionicBatteryDisplayer.ElectrobankState electrobankState = ((component == null) ? BionicBatteryDisplayer.ElectrobankState.Damaged : ((component.Charge <= 0f) ? BionicBatteryDisplayer.ElectrobankState.Depleated : BionicBatteryDisplayer.ElectrobankState.Charged));
					string iconForState = this.GetIconForState(electrobankState);
					float num2 = ((component == null) ? 0f : component.Charge);
					stringBuilder.Append("\n");
					stringBuilder.Append("    • ");
					stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_ROW, iconForState, gameObject.GetProperName(), GameUtil.GetFormattedJoules(num2, "F1", GameUtil.TimeSlice.None));
				}
			}
			if (electrobankCount < electrobankCountCapacity)
			{
				for (int j = 0; j < electrobankCountCapacity - electrobankCount; j++)
				{
					stringBuilder.Append("\n");
					stringBuilder.Append("    • ");
					stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ELECTROBANK_EMPTY_ROW, this.GetIconForState(BionicBatteryDisplayer.ElectrobankState.Unexistent));
				}
			}
		}
		stringBuilder.Append("\n\n");
		stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.CURRENT_WATTAGE_LABEL, this.formatter.GetFormattedValue(num, this.formatter.DeltaTimeSlice));
		if (smi != null)
		{
			StringBuilder stringBuilder2 = GlobalStringBuilderPool.Alloc();
			stringBuilder2.Append("<b>+</b>");
			GameUtil.AppendFormattedWattage(stringBuilder2, smi.GetBaseWattage(), GameUtil.WattageFormatterUnit.Automatic, true);
			stringBuilder.Append("\n");
			stringBuilder.Append("    • ");
			stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, DUPLICANTS.MODIFIERS.BIONIC_WATTS.BASE_NAME, stringBuilder2.ToString());
			stringBuilder2.Clear();
			float num3 = 0f;
			foreach (BionicBatteryMonitor.WattageModifier wattageModifier in smi.Modifiers)
			{
				if (wattageModifier.value != 0f)
				{
					stringBuilder.Append("\n");
					stringBuilder.Append("    • ");
					stringBuilder.Append(wattageModifier.name);
				}
				else if (wattageModifier.potentialValue > 0f)
				{
					stringBuilder2.Append("\n");
					stringBuilder2.Append("    • ");
					stringBuilder2.Append(wattageModifier.name);
					num3 += wattageModifier.potentialValue;
				}
			}
			if (stringBuilder2.Length != 0)
			{
				stringBuilder.Append("\n\n");
				stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.POTENTIAL_EXTRA_WATTAGE_LABEL, this.formatter.GetFormattedValue(num3, this.formatter.DeltaTimeSlice));
				stringBuilder.Append(stringBuilder2.ToString());
			}
			GlobalStringBuilderPool.Free(stringBuilder2);
		}
		global::Debug.Assert(instance.deltaAttribute.Modifiers.Count <= 0, "Bionic Battery Displayer has found an invalid AttributeModifier. This particular Amount should not use AttributeModifiers, instead, use BionicBatteryMonitor.Instance.Modifiers");
		float num4 = ((num == 0f) ? 0f : (smi.CurrentCharge / num));
		stringBuilder.Append("\n\n");
		stringBuilder.AppendFormat(DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.ESTIMATED_LIFE_TIME_REMAINING, GameUtil.GetFormattedCycles(num4, "F1", false));
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
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
