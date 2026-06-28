using System;
using Klei.AI;

namespace Database
{
	public class Attributes : ResourceSet<Klei.AI.Attribute>
	{
		public Attributes(ResourceSet parent)
			: base("Attributes", parent)
		{
			this.Construction = base.Add(new Klei.AI.Attribute("Construction", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Construction.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Digging = base.Add(new Klei.AI.Attribute("Digging", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Digging.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Machinery = base.Add(new Klei.AI.Attribute("Machinery", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Machinery.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Athletics = base.Add(new Klei.AI.Attribute("Athletics", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Athletics.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Learning = base.Add(new Klei.AI.Attribute("Learning", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Learning.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Cooking = base.Add(new Klei.AI.Attribute("Cooking", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Cooking.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Art = base.Add(new Klei.AI.Attribute("Art", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Art.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Strength = base.Add(new Klei.AI.Attribute("Strength", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Strength.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Caring = base.Add(new Klei.AI.Attribute("Caring", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Caring.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Botanist = base.Add(new Klei.AI.Attribute("Botanist", true, Klei.AI.Attribute.Display.Skill, true, 0f));
			this.Botanist.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.PowerTinker = base.Add(new Klei.AI.Attribute("PowerTinker", true, Klei.AI.Attribute.Display.Normal, true, 0f));
			this.PowerTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.FarmTinker = base.Add(new Klei.AI.Attribute("FarmTinker", true, Klei.AI.Attribute.Display.Normal, true, 0f));
			this.FarmTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Immunity = base.Add(new Klei.AI.Attribute("Immunity", true, Klei.AI.Attribute.Display.Details, false, 0f));
			this.Immunity.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.ThermalConductivityBarrier = base.Add(new Klei.AI.Attribute("ThermalConductivityBarrier", false, Klei.AI.Attribute.Display.Normal, false, 0f));
			this.ThermalConductivityBarrier.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Distance, GameUtil.TimeSlice.None));
			this.Insulation = base.Add(new Klei.AI.Attribute("Insulation", false, Klei.AI.Attribute.Display.General, true, 0f));
			this.Decor = base.Add(new Klei.AI.Attribute("Decor", false, Klei.AI.Attribute.Display.General, false, 0f));
			this.FoodQuality = base.Add(new Klei.AI.Attribute("FoodQuality", false, Klei.AI.Attribute.Display.General, false, 0f));
			this.FoodQuality.SetFormatter(new FoodQualityAttributeFormatter());
			this.ScaldingThreshold = base.Add(new Klei.AI.Attribute("ScaldingThreshold", false, Klei.AI.Attribute.Display.General, false, 0f));
			this.ScaldingThreshold.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			this.GeneratorOutput = base.Add(new Klei.AI.Attribute("GeneratorOutput", false, Klei.AI.Attribute.Display.General, false, 0f));
			this.GeneratorOutput.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
			this.MachinerySpeed = base.Add(new Klei.AI.Attribute("MachinerySpeed", false, Klei.AI.Attribute.Display.General, false, 1f));
			this.MachinerySpeed.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
			this.MachinerySpeed.SetFormatter(new PercentAttributeFormatter());
			this.DecorExpectation = base.Add(new Klei.AI.Attribute("DecorExpectation", false, Klei.AI.Attribute.Display.Expectation, false, 0f));
			this.FoodExpectation = base.Add(new Klei.AI.Attribute("FoodExpectation", false, Klei.AI.Attribute.Display.Expectation, false, 0f));
			this.FoodExpectation.SetFormatter(new FoodQualityAttributeFormatter());
			this.RoomTemperaturePreference = base.Add(new Klei.AI.Attribute("RoomTemperaturePreference", false, Klei.AI.Attribute.Display.Normal, false, 0f));
			this.AirConsumptionRate = base.Add(new Klei.AI.Attribute("AirConsumptionRate", false, Klei.AI.Attribute.Display.Normal, false, 0f));
			this.AirConsumptionRate.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			this.MaxUnderwaterTravelCost = base.Add(new Klei.AI.Attribute("MaxUnderwaterTravelCost", false, Klei.AI.Attribute.Display.Normal, false, 0f));
			this.ToiletEfficiency = base.Add(new Klei.AI.Attribute("ToiletEfficiency", false, Klei.AI.Attribute.Display.Details, false, 0f));
			this.ToiletEfficiency.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			this.Sneezyness = base.Add(new Klei.AI.Attribute("Sneezyness", false, Klei.AI.Attribute.Display.Details, false, 0f));
			this.DiseaseCureSpeed = base.Add(new Klei.AI.Attribute("DiseaseCureSpeed", false, Klei.AI.Attribute.Display.Normal, false, 0f));
			this.DiseaseCureSpeed.BaseValue = 1f;
			this.DiseaseCureSpeed.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			this.DoctoredLevel = base.Add(new Klei.AI.Attribute("DoctoredLevel", false, Klei.AI.Attribute.Display.Never, false, 0f));
			this.CarryAmount = base.Add(new Klei.AI.Attribute("CarryAmount", false, Klei.AI.Attribute.Display.Details, false, 0f));
			this.CarryAmount.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None));
		}

		public Klei.AI.Attribute Construction;

		public Klei.AI.Attribute Digging;

		public Klei.AI.Attribute Machinery;

		public Klei.AI.Attribute Athletics;

		public Klei.AI.Attribute Learning;

		public Klei.AI.Attribute Cooking;

		public Klei.AI.Attribute Caring;

		public Klei.AI.Attribute Strength;

		public Klei.AI.Attribute Art;

		public Klei.AI.Attribute Botanist;

		public Klei.AI.Attribute PowerTinker;

		public Klei.AI.Attribute FarmTinker;

		public Klei.AI.Attribute Immunity;

		public Klei.AI.Attribute Insulation;

		public Klei.AI.Attribute ThermalConductivityBarrier;

		public Klei.AI.Attribute Decor;

		public Klei.AI.Attribute FoodQuality;

		public Klei.AI.Attribute ScaldingThreshold;

		public Klei.AI.Attribute GeneratorOutput;

		public Klei.AI.Attribute MachinerySpeed;

		public Klei.AI.Attribute DecorExpectation;

		public Klei.AI.Attribute FoodExpectation;

		public Klei.AI.Attribute RoomTemperaturePreference;

		public Klei.AI.Attribute AirConsumptionRate;

		public Klei.AI.Attribute MaxUnderwaterTravelCost;

		public Klei.AI.Attribute ToiletEfficiency;

		public Klei.AI.Attribute Sneezyness;

		public Klei.AI.Attribute DiseaseCureSpeed;

		public Klei.AI.Attribute DoctoredLevel;

		public Klei.AI.Attribute CarryAmount;
	}
}
