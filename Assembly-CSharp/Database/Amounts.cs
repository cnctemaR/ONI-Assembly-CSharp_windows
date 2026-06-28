using System;
using Klei.AI;
using UnityEngine;

namespace Database
{
	public class Amounts : ResourceSet<Amount>
	{
		public void Load(TextAsset file)
		{
			ResourceLoader<Amounts.AmountInfo> resourceLoader = new ResourceLoader<Amounts.AmountInfo>(file);
			foreach (Amounts.AmountInfo amountInfo in resourceLoader)
			{
				this.CreateAmount(amountInfo.Id, amountInfo.Min, amountInfo.Max, amountInfo.InitialMin, amountInfo.InitialMax, amountInfo.ShowMax, amountInfo.Units, amountInfo.VisualDeltaThreshold, true, "STRINGS.DUPLICANTS.STATS");
			}
			this.Decor = this.CreateAmount("Decor", 0f, 100f, 50f, 50f, true, Units.Flat, 0.1f, true, "STRINGS.DUPLICANTS.STATS");
			this.Decor.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerSecond));
			this.Maturity = this.CreateAmount("Maturity", 0f, 0f, 0f, 0f, true, Units.Flat, 0.0009166667f, true, "STRINGS.CREATURES.STATS");
			this.Maturity.SetDisplayer(new MaturityDisplayer());
			this.OldAge = this.CreateAmount("OldAge", 0f, 2400f, 0f, 2400f, false, Units.Flat, 0f, false, "STRINGS.CREATURES.STATS");
			this.Fertilization = this.CreateAmount("Fertilization", 0f, 100f, 0f, 100f, true, Units.Flat, 0.1675f, true, "STRINGS.CREATURES.STATS");
			this.Fertilization.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			this.HitPoints = this.CreateAmount("HitPoints", 0f, 0f, 0f, 0f, true, Units.Flat, 0.1675f, true, "STRINGS.DUPLICANTS.STATS");
			this.HitPoints.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerSecond));
			this.BindAmounts();
		}

		public Amount CreateAmount(string id, float min, float max, float initial_min, float initial_max, bool show_max, Units units, float delta_threshold, bool show_in_ui, string string_root)
		{
			string text = Strings.Get(string.Format("{1}.{0}.NAME", id.ToUpper(), string_root.ToUpper()));
			string text2 = Strings.Get(string.Format("{1}.{0}.TOOLTIP", id.ToUpper(), string_root.ToUpper()));
			Klei.AI.Attribute attribute = new Klei.AI.Attribute(id + "Min", "Minimum" + text, string.Empty, string.Empty, min, false, false);
			Klei.AI.Attribute attribute2 = new Klei.AI.Attribute(id + "Max", "Maximum" + text, string.Empty, string.Empty, max, false, false);
			string text3 = id + "Delta";
			string text4 = Strings.Get(string.Format("STRINGS.DUPLICANTS.ATTRIBUTES.{0}.NAME", text3.ToUpper()));
			Klei.AI.Attribute attribute3 = new Klei.AI.Attribute(text3, text4, string.Empty, string.Empty, 0f, false, false);
			Amount amount = new Amount(id, text, text2, initial_min, initial_max, attribute, attribute2, attribute3, show_max, units, delta_threshold, show_in_ui);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(attribute3);
			base.Add(amount);
			return amount;
		}

		private void BindAmounts()
		{
			this.Stamina = base.Get("Stamina");
			this.Stamina.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			this.Calories = base.Get("Calories");
			this.Calories.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerSecond));
			this.Temperature = base.Get("Temperature");
			this.Temperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.ExternalTemperature = base.Get("ExternalTemperature");
			this.ExternalTemperature.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
			this.Breath = base.Get("Breath");
			this.Breath.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			this.Stress = base.Get("Stress");
			this.Stress.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			this.Toxicity = base.Get("Toxicity");
			this.Toxicity.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
			this.Bladder = base.Get("Bladder");
			this.Bladder.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
		}

		public Amount Stamina;

		public Amount Calories;

		public Amount HitPoints;

		public Amount Temperature;

		public Amount ExternalTemperature;

		public Amount Breath;

		public Amount Stress;

		public Amount Toxicity;

		public Amount Bladder;

		public Amount Decor;

		public Amount Maturity;

		public Amount OldAge;

		public Amount Fertilization;

		public class AmountInfo : Resource
		{
			public float Min;

			public float Max;

			public float InitialMin;

			public float InitialMax;

			public bool ShowMax;

			public Units Units;

			public float VisualDeltaThreshold;
		}
	}
}
