using System;
using Klei.AI;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public void Load()
		{
			this.Stamina = this.CreateAmount("Stamina", 0f, 100f, false, Units.Flat, 0.35f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stamina", "attribute_stamina");
			this.Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Calories = this.CreateAmount("Calories", 0f, 0f, false, Units.Flat, 4000f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_calories", "attribute_calories");
			this.Calories.SetDisplayer(new CaloriesDisplayer());
			this.Temperature = this.CreateAmount("Temperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_temperature", null);
			this.Temperature.SetDisplayer(new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.ExternalTemperature = this.CreateAmount("ExternalTemperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS", null, null);
			this.ExternalTemperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond, null));
			this.Breath = this.CreateAmount("Breath", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_breath", null);
			this.Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.Stress = this.CreateAmount("Stress", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_stress", "attribute_stress");
			this.Stress.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Toxicity = this.CreateAmount("Toxicity", 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", null, null);
			this.Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Bladder = this.CreateAmount("Bladder", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_bladder", null);
			this.Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Decor = this.CreateAmount("Decor", -1000f, 1000f, false, Units.Flat, 0.016666668f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_decor", null);
			this.Decor.SetDisplayer(new DecorDisplayer());
			this.Maturity = this.CreateAmount("Maturity", 0f, 0f, true, Units.Flat, 0.0009166667f, true, "STRINGS.CREATURES.STATS", "ui_icon_maturity", null);
			this.Maturity.SetDisplayer(new MaturityDisplayer());
			this.OldAge = this.CreateAmount("OldAge", 0f, 2400f, false, Units.Flat, 0f, false, "STRINGS.CREATURES.STATS", null, null);
			this.Fertilization = this.CreateAmount("Fertilization", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			this.Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.Fertility = this.CreateAmount("Fertility", 0f, 100f, true, Units.Flat, 0.008375f, true, "STRINGS.CREATURES.STATS", "ui_icon_fertility", null);
			this.Fertility.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Wildness = this.CreateAmount("Wildness", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_wildness", null);
			this.Wildness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Incubation = this.CreateAmount("Incubation", 0f, 100f, true, Units.Flat, 0.01675f, true, "STRINGS.CREATURES.STATS", "ui_icon_incubation", null);
			this.Incubation.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Viability = this.CreateAmount("Viability", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			this.Viability.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Age = this.CreateAmount("Age", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_age", null);
			this.Age.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null));
			this.Irrigation = this.CreateAmount("Irrigation", 0f, 1f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", null, null);
			this.Irrigation.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.HitPoints = this.CreateAmount("HitPoints", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_hitpoints", "attribute_hitpoints");
			this.HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, null));
			this.ImmuneLevel = this.CreateAmount("ImmuneLevel", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS", "ui_icon_immunelevel", "attribute_immunelevel");
			this.ImmuneLevel.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Rot = this.CreateAmount("Rot", 0f, 0f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			this.Rot.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.AirPressure = this.CreateAmount("AirPressure", 0f, 1E+09f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			this.AirPressure.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond, null));
			this.Illumination = this.CreateAmount("Illumination", 0f, 1f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS", null, null);
			this.Illumination.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None, null));
			this.ScaleGrowth = this.CreateAmount("ScaleGrowth", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS", "ui_icon_scale_growth", null);
			this.ScaleGrowth.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root, string uiSprite = null, string thoughtSprite = null)
		{
			string text = Strings.Get(string.Format("{1}.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string text2 = Strings.Get(string.Format("{1}.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			Klei.AI.Attribute attribute = new Klei.AI.Attribute(id + "Min", "Minimum" + text, string.Empty, string.Empty, min, Klei.AI.Attribute.Display.Normal, false, null, null);
			Klei.AI.Attribute attribute2 = new Klei.AI.Attribute(id + "Max", "Maximum" + text, string.Empty, string.Empty, max, Klei.AI.Attribute.Display.Normal, false, null, null);
			string text3 = id + "Delta";
			string text4 = Strings.Get(string.Format("STRINGS.DUPLICANTS.ATTRIBUTES.{0}.NAME", text3.ToUpper()));
			Klei.AI.Attribute attribute3 = new Klei.AI.Attribute(text3, text4, string.Empty, string.Empty, 0f, Klei.AI.Attribute.Display.Normal, false, null, null);
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

		public Amount ExternalTemperature;

		public Amount Breath;

		public Amount Stress;

		public Amount Toxicity;

		public Amount Bladder;

		public Amount Decor;

		public Amount Temperature;

		public Amount HitPoints;

		public Amount AirPressure;

		public Amount Maturity;

		public Amount OldAge;

		public Amount Age;

		public Amount Fertilization;

		public Amount Illumination;

		public Amount Irrigation;

		public Amount CreatureCalories;

		public Amount Fertility;

		public Amount Viability;

		public Amount Wildness;

		public Amount Incubation;

		public Amount ScaleGrowth;

		public Amount Rot;
	}
}
