using System;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public void Load()
		{
			this.Stamina = this.CreateAmount("Stamina", 0f, 100f, false, Units.Flat, 0.35f, true, "STRINGS.DUPLICANTS", "ui_icon_stamina", "attribute_stamina", "mod_stamina");
			this.Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Calories = this.CreateAmount("Calories", 0f, 0f, false, Units.Flat, 4000f, true, "STRINGS.DUPLICANTS", "ui_icon_calories", "attribute_calories", "mod_calories");
			this.Calories.SetDisplayer(new CaloriesDisplayer());
			this.Breath = this.CreateAmount("Breath", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_breath", null, "mod_breath");
			this.Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Stress = this.CreateAmount("Stress", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_stress", "attribute_stress", "mod_stress");
			this.Stress.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Toxicity = this.CreateAmount("Toxicity", 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", null, null, null);
			this.Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Bladder = this.CreateAmount("Bladder", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_bladder", null, "mod_bladder");
			this.Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Decor = this.CreateAmount("Decor", -1000f, 1000f, false, Units.Flat, 0.016666668f, true, "STRINGS.DUPLICANTS", "ui_icon_decor", null, "mod_decor");
			this.Decor.SetDisplayer(new DecorDisplayer());
			this.RadiationBalance = this.CreateAmount("RadiationBalance", 0f, 10000f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_radiation", null, "mod_health");
			this.RadiationBalance.SetDisplayer(new RadiationBalanceDisplayer());
			this.Temperature = this.CreateAmount("Temperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_temperature", null, null);
			this.Temperature.SetDisplayer(new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.CritterTemperature = this.CreateAmount("CritterTemperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.CREATURES", "ui_icon_temperature", null, null);
			this.CritterTemperature.SetDisplayer(new CritterTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.HitPoints = this.CreateAmount("HitPoints", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS", "ui_icon_hitpoints", "attribute_hitpoints", "mod_health");
			this.HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Possessive));
			this.AirPressure = this.CreateAmount("AirPressure", 0f, 1E+09f, false, Units.Flat, 0f, true, "STRINGS.CREATURES", null, null, null);
			this.AirPressure.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Maturity = this.CreateAmount("Maturity", 0f, 0f, true, Units.Flat, 0.0009166667f, true, "STRINGS.CREATURES", "ui_icon_maturity", null, null);
			this.Maturity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Cycles, GameUtil.TimeSlice.None, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Maturity2 = this.CreateAmount("Maturity2", 0f, 0f, true, Units.Flat, 0.0009166667f, true, false, "STRINGS.CREATURES", "ui_icon_maturity", null, null);
			this.Maturity2.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Cycles, GameUtil.TimeSlice.None, null, GameUtil.IdentityDescriptorTense.Normal));
			this.OldAge = this.CreateAmount("OldAge", 0f, 0f, false, Units.Flat, 0f, false, "STRINGS.CREATURES", null, null, null);
			this.Fertilization = this.CreateAmount("Fertilization", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", null, null, null);
			this.Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Fertility = this.CreateAmount("Fertility", 0f, 100f, true, Units.Flat, 0.008375f, true, "STRINGS.CREATURES", "ui_icon_fertility", null, null);
			this.Fertility.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Wildness = this.CreateAmount("Wildness", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_wildness", null, null);
			this.Wildness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Incubation = this.CreateAmount("Incubation", 0f, 100f, true, Units.Flat, 0.01675f, true, "STRINGS.CREATURES", "ui_icon_incubation", null, null);
			this.Incubation.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Viability = this.CreateAmount("Viability", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_viability", null, null);
			this.Viability.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.PowerCharge = this.CreateAmount("PowerCharge", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", null, null, null);
			this.PowerCharge.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Age = this.CreateAmount("Age", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_age", null, null);
			this.Age.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal));
			this.Irrigation = this.CreateAmount("Irrigation", 0f, 1f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", null, null, null);
			this.Irrigation.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.ImmuneLevel = this.CreateAmount("ImmuneLevel", 0f, DUPLICANTSTATS.STANDARD.BaseStats.IMMUNE_LEVEL_MAX, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS", "ui_icon_immunelevel", "attribute_immunelevel", null);
			this.ImmuneLevel.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Rot = this.CreateAmount("Rot", 0f, 0f, false, Units.Flat, 0f, true, "STRINGS.CREATURES", null, null, null);
			this.Rot.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Illumination = this.CreateAmount("Illumination", 0f, 1f, false, Units.Flat, 0f, true, "STRINGS.CREATURES", null, null, null);
			this.Illumination.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None, null, GameUtil.IdentityDescriptorTense.Normal));
			this.ScaleGrowth = this.CreateAmount("ScaleGrowth", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_scale_growth", null, null);
			this.ScaleGrowth.SetDisplayer(new ScaleGrowthDisplayer(GameUtil.TimeSlice.PerCycle));
			this.MilkProduction = this.CreateAmount("MilkProduction", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_milk_production", null, null);
			this.MilkProduction.SetDisplayer(new MilkProductionDisplayer(GameUtil.TimeSlice.PerCycle));
			this.ElementGrowth = this.CreateAmount("ElementGrowth", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_scale_growth", null, null);
			this.ElementGrowth.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Beckoning = this.CreateAmount("Beckoning", 0f, 100f, true, Units.Flat, 100.5f, true, "STRINGS.CREATURES", "ui_icon_moo", null, null);
			this.Beckoning.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.BionicOxygenTank = this.CreateAmount("BionicOxygenTank", 0f, BionicOxygenTankMonitor.OXYGEN_TANK_CAPACITY_KG, true, Units.Flat, 60f, true, "STRINGS.DUPLICANTS", "ui_icon_oxygentank", null, null);
			this.BionicOxygenTank.SetDisplayer(new BionicOxygenTankDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			this.BionicOxygenTank.debugSetValue = delegate(AmountInstance instance, float val)
			{
				BionicOxygenTankMonitor.Instance smi = instance.gameObject.GetSMI<BionicOxygenTankMonitor.Instance>();
				if (smi == null)
				{
					instance.SetValue(val);
					return;
				}
				float availableOxygen = smi.AvailableOxygen;
				if (val >= availableOxygen)
				{
					float num = val - availableOxygen;
					smi.AddGas(SimHashes.Oxygen, num, 6282.4497f, byte.MaxValue, 0);
					return;
				}
				float num2 = Mathf.Min(availableOxygen - val, availableOxygen);
				float num3;
				SimUtil.DiseaseInfo diseaseInfo;
				float num4;
				smi.storage.ConsumeAndGetDisease(GameTags.Breathable, num2, out num3, out diseaseInfo, out num4);
			};
			this.BionicInternalBattery = this.CreateAmount("BionicInternalBattery", 0f, 480000f, true, Units.Flat, 4000f, true, "STRINGS.DUPLICANTS", "ui_icon_battery", null, null);
			this.BionicInternalBattery.SetDisplayer(new BionicBatteryDisplayer());
			this.BionicInternalBattery.debugSetValue = delegate(AmountInstance instance, float val)
			{
				BionicBatteryMonitor.Instance smi2 = instance.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
				if (smi2 == null)
				{
					instance.SetValue(val);
					return;
				}
				float currentCharge = smi2.CurrentCharge;
				if (val >= currentCharge)
				{
					float num5 = val - currentCharge;
					smi2.DebugAddCharge(num5);
					return;
				}
				float num6 = currentCharge - val;
				smi2.ConsumePower(num6);
			};
			this.BionicOil = this.CreateAmount("BionicOil", 0f, 200f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_liquid", null, null);
			this.BionicOil.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerCycle, null, GameUtil.IdentityDescriptorTense.Normal));
			this.BionicGunk = this.CreateAmount("BionicGunk", 0f, GunkMonitor.GUNK_CAPACITY, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_gunk", null, null);
			this.BionicGunk.SetDisplayer(new BionicGunkDisplayer(GameUtil.TimeSlice.PerCycle));
			this.InternalBattery = this.CreateAmount("InternalBattery", 0f, 0f, true, Units.Flat, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery", null, null);
			this.InternalBattery.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.InternalChemicalBattery = this.CreateAmount("InternalChemicalBattery", 0f, 0f, true, Units.Flat, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery", null, null);
			this.InternalChemicalBattery.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.InternalBioBattery = this.CreateAmount("InternalBioBattery", 0f, 0f, true, Units.Flat, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery", null, null);
			this.InternalBioBattery.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
			this.InternalElectroBank = this.CreateAmount("InternalElectroBank", 0f, 0f, true, Units.Flat, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery", null, null);
			this.InternalElectroBank.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond, null, GameUtil.IdentityDescriptorTense.Normal));
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null)
		{
			return this.CreateAmount(id, min, max, show_max, units, delta_threshold, show_in_ui, true, string_root, uiSprite, thoughtSprite, uiFullColourSprite);
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, bool show_delta_in_ui, string string_root, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null)
		{
			string text = Strings.Get(string.Format("{1}.STATS.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string text2 = Strings.Get(string.Format("{1}.STATS.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			global::Klei.AI.Attribute.Display display = (show_in_ui ? global::Klei.AI.Attribute.Display.Normal : global::Klei.AI.Attribute.Display.Never);
			global::Klei.AI.Attribute.Display display2 = (show_delta_in_ui ? global::Klei.AI.Attribute.Display.Normal : global::Klei.AI.Attribute.Display.Never);
			string text3 = id + "Min";
			StringEntry stringEntry;
			string text4 = (Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.NAME", text3.ToUpper(), string_root)), out stringEntry) ? stringEntry.String : ("Minimum" + text));
			StringEntry stringEntry2;
			string text5 = (Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.DESC", text3.ToUpper(), string_root)), out stringEntry2) ? stringEntry2.String : ("Minimum" + text));
			global::Klei.AI.Attribute attribute = new global::Klei.AI.Attribute(id + "Min", text4, "", text5, min, display, false, null, null, uiFullColourSprite);
			string text6 = id + "Max";
			StringEntry stringEntry3;
			string text7 = (Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.NAME", text6.ToUpper(), string_root)), out stringEntry3) ? stringEntry3.String : ("Maximum" + text));
			StringEntry stringEntry4;
			string text8 = (Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.DESC", text6.ToUpper(), string_root)), out stringEntry4) ? stringEntry4.String : ("Maximum" + text));
			global::Klei.AI.Attribute attribute2 = new global::Klei.AI.Attribute(id + "Max", text7, "", text8, max, display, false, null, null, uiFullColourSprite);
			string text9 = id + "Delta";
			string text10 = Strings.Get(string.Format("{1}.ATTRIBUTES.{0}.NAME", text9.ToUpper(), string_root));
			string text11 = Strings.Get(string.Format("{1}.ATTRIBUTES.{0}.DESC", text9.ToUpper(), string_root));
			global::Klei.AI.Attribute attribute3 = new global::Klei.AI.Attribute(text9, text10, "", text11, 0f, display2, false, null, null, uiFullColourSprite);
			Amount amount = new Amount(id, text, text2, attribute, attribute2, attribute3, show_max, units, delta_threshold, show_in_ui, uiSprite, thoughtSprite);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(attribute3);
			base.Add(amount);
			return amount;
		}

		public Amount Stamina;

		public Amount Calories;

		public Amount ImmuneLevel;

		public Amount Breath;

		public Amount Stress;

		public Amount Toxicity;

		public Amount Bladder;

		public Amount Decor;

		public Amount RadiationBalance;

		public Amount BionicOxygenTank;

		public Amount BionicOil;

		public Amount BionicGunk;

		public Amount BionicInternalBattery;

		public Amount Temperature;

		public Amount CritterTemperature;

		public Amount HitPoints;

		public Amount AirPressure;

		public Amount Maturity;

		public Amount Maturity2;

		public Amount OldAge;

		public Amount Age;

		public Amount Fertilization;

		public Amount Illumination;

		public Amount Irrigation;

		public Amount Fertility;

		public Amount Viability;

		public Amount PowerCharge;

		public Amount Wildness;

		public Amount Incubation;

		public Amount ScaleGrowth;

		public Amount ElementGrowth;

		public Amount Beckoning;

		public Amount MilkProduction;

		public Amount InternalBattery;

		public Amount InternalChemicalBattery;

		public Amount InternalBioBattery;

		public Amount InternalElectroBank;

		public Amount Rot;
	}
}
