using System;
using Klei.AI;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public void Load()
		{
			this.Stamina = this.CreateAmount("Stamina", 0f, 100f, false, Units.Flat, 0.35f, true, "STRINGS.DUPLICANTS.STATS");
			this.Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Calories = this.CreateAmount("Calories", 0f, 0f, false, Units.Flat, 4000f, true, "STRINGS.DUPLICANTS.STATS");
			this.Calories.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle, null));
			this.Temperature = this.CreateAmount("Temperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.Temperature.SetDisplayer(new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.ExternalTemperature = this.CreateAmount("ExternalTemperature", 0f, 10000f, false, Units.Kelvin, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.ExternalTemperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond, null));
			this.Breath = this.CreateAmount("Breath", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.Stress = this.CreateAmount("Stress", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.Stress.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Toxicity = this.CreateAmount("Toxicity", 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Bladder = this.CreateAmount("Bladder", 0f, 100f, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS.STATS");
			this.Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));
			this.Decor = this.CreateAmount("Decor", -1000f, 1000f, false, Units.Flat, 0.1f, true, "STRINGS.DUPLICANTS.STATS");
			this.Decor.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerSecond, null));
			this.Maturity = this.CreateAmount("Maturity", 0f, 0f, true, Units.Flat, 0.0009166667f, true, "STRINGS.CREATURES.STATS");
			this.Maturity.SetDisplayer(new MaturityDisplayer());
			this.OldAge = this.CreateAmount("OldAge", 0f, 2400f, false, Units.Flat, 0f, false, "STRINGS.CREATURES.STATS");
			this.Fertilization = this.CreateAmount("Fertilization", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.Happiness = this.CreateAmount("Happiness", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Happiness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Fertility = this.CreateAmount("Fertility", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Fertility.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Wildness = this.CreateAmount("Wildness", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Wildness.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Incubation = this.CreateAmount("Incubation", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Incubation.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Irrigation = this.CreateAmount("Irrigation", 0f, 1f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Irrigation.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond, null));
			this.HitPoints = this.CreateAmount("HitPoints", 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS");
			this.HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerSecond, null));
			this.ImmuneLevel = this.CreateAmount("ImmuneLevel", 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS");
			this.ImmuneLevel.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.Rot = this.CreateAmount("Rot", 0f, 0f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS");
			this.Rot.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
			this.AirPressure = this.CreateAmount("AirPressure", 0f, 1E+09f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS");
			this.AirPressure.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond, null));
			this.Illumination = this.CreateAmount("Illumination", 0f, 1f, false, Units.Flat, 0f, true, "STRINGS.CREATURES.STATS");
			this.Illumination.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None, null));
		}

		public Amount CreateAmount(string id, float min, float max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root)
		{
			string text = Strings.Get(string.Format("{1}.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string text2 = Strings.Get(string.Format("{1}.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			Klei.AI.Attribute attribute = new Klei.AI.Attribute(id + "Min", "Minimum" + text, string.Empty, string.Empty, min, Klei.AI.Attribute.Display.Normal, false);
			Klei.AI.Attribute attribute2 = new Klei.AI.Attribute(id + "Max", "Maximum" + text, string.Empty, string.Empty, max, Klei.AI.Attribute.Display.Normal, false);
			string text3 = id + "Delta";
			string text4 = Strings.Get(string.Format("STRINGS.DUPLICANTS.ATTRIBUTES.{0}.NAME", text3.ToUpper()));
			Klei.AI.Attribute attribute3 = new Klei.AI.Attribute(text3, text4, string.Empty, string.Empty, 0f, Klei.AI.Attribute.Display.Normal, false);
			Amount amount = new Amount(id, text, text2, attribute, attribute2, attribute3, show_max, units, delta_threshold, show_in_ui);
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

		public Amount Fertilization;

		public Amount Illumination;

		public Amount Irrigation;

		public Amount CreatureCalories;

		public Amount Happiness;

		public Amount Fertility;

		public Amount Wildness;

		public Amount Incubation;

		public Amount Rot;
	}
}
