using System;
using Klei.AI;

namespace Database
{
	public class Attributes : ResourceSet<Klei.AI.Attribute>
	{
		public Attributes(ResourceSet parent)
			: base("Attributes", parent)
		{
			this.Construction = base.Add(new Klei.AI.Attribute("Construction", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_construction", null));
			this.Construction.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Digging = base.Add(new Klei.AI.Attribute("Digging", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_excavation", null));
			this.Digging.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Machinery = base.Add(new Klei.AI.Attribute("Machinery", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_machinery", null));
			this.Machinery.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Athletics = base.Add(new Klei.AI.Attribute("Athletics", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_athletics", null));
			this.Athletics.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Learning = base.Add(new Klei.AI.Attribute("Learning", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_science", null));
			this.Learning.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Cooking = base.Add(new Klei.AI.Attribute("Cooking", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_cusine", null));
			this.Cooking.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Caring = base.Add(new Klei.AI.Attribute("Caring", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_medicine", null));
			this.Caring.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Strength = base.Add(new Klei.AI.Attribute("Strength", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_strength", null));
			this.Strength.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Art = base.Add(new Klei.AI.Attribute("Art", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_creativity", null));
			this.Art.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Botanist = base.Add(new Klei.AI.Attribute("Botanist", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_agriculture", null));
			this.Botanist.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Ranching = base.Add(new Klei.AI.Attribute("Ranching", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, "mod_husbandry", null));
			this.Ranching.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.PowerTinker = base.Add(new Klei.AI.Attribute("PowerTinker", true, Klei.AI.Attribute.Display.Normal, true, 0f, null, null, null, null));
			this.PowerTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.FarmTinker = base.Add(new Klei.AI.Attribute("FarmTinker", true, Klei.AI.Attribute.Display.Normal, true, 0f, null, null, null, null));
			this.FarmTinker.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			if (DlcManager.IsExpansion1Active())
			{
				this.SpaceNavigation = base.Add(new Klei.AI.Attribute("SpaceNavigation", true, Klei.AI.Attribute.Display.Skill, true, 0f, null, null, null, null));
				this.SpaceNavigation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			}
			else
			{
				this.SpaceNavigation = base.Add(new Klei.AI.Attribute("SpaceNavigation", true, Klei.AI.Attribute.Display.Normal, true, 0f, null, null, null, null));
				this.SpaceNavigation.SetFormatter(new PercentAttributeFormatter());
			}
			this.Immunity = base.Add(new Klei.AI.Attribute("Immunity", true, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, null));
			this.Immunity.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.ThermalConductivityBarrier = base.Add(new Klei.AI.Attribute("ThermalConductivityBarrier", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, null));
			this.ThermalConductivityBarrier.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Distance, GameUtil.TimeSlice.None));
			this.Insulation = base.Add(new Klei.AI.Attribute("Insulation", false, Klei.AI.Attribute.Display.General, true, 0f, null, null, null, null));
			this.Luminescence = base.Add(new Klei.AI.Attribute("Luminescence", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.Decor = base.Add(new Klei.AI.Attribute("Decor", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.Decor.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.FoodQuality = base.Add(new Klei.AI.Attribute("FoodQuality", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.FoodQuality.SetFormatter(new FoodQualityAttributeFormatter());
			this.ScaldingThreshold = base.Add(new Klei.AI.Attribute("ScaldingThreshold", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.ScaldingThreshold.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			this.ScoldingThreshold = base.Add(new Klei.AI.Attribute("ScoldingThreshold", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.ScoldingThreshold.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			this.GeneratorOutput = base.Add(new Klei.AI.Attribute("GeneratorOutput", false, Klei.AI.Attribute.Display.General, false, 0f, null, null, null, null));
			this.GeneratorOutput.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
			this.MachinerySpeed = base.Add(new Klei.AI.Attribute("MachinerySpeed", false, Klei.AI.Attribute.Display.General, false, 1f, null, null, null, null));
			this.MachinerySpeed.SetFormatter(new PercentAttributeFormatter());
			this.RadiationResistance = base.Add(new Klei.AI.Attribute("RadiationResistance", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, DlcManager.AVAILABLE_EXPANSION1_ONLY));
			this.RadiationResistance.SetFormatter(new PercentAttributeFormatter());
			this.RadiationRecovery = base.Add(new Klei.AI.Attribute("RadiationRecovery", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, DlcManager.AVAILABLE_EXPANSION1_ONLY));
			this.RadiationRecovery.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Radiation, GameUtil.TimeSlice.PerCycle));
			this.DecorExpectation = base.Add(new Klei.AI.Attribute("DecorExpectation", false, Klei.AI.Attribute.Display.Expectation, false, 0f, null, null, null, null));
			this.DecorExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.FoodExpectation = base.Add(new Klei.AI.Attribute("FoodExpectation", false, Klei.AI.Attribute.Display.Expectation, false, 0f, null, null, null, null));
			this.FoodExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.RoomTemperaturePreference = base.Add(new Klei.AI.Attribute("RoomTemperaturePreference", false, Klei.AI.Attribute.Display.Normal, false, 0f, null, null, null, null));
			this.RoomTemperaturePreference.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
			this.QualityOfLifeExpectation = base.Add(new Klei.AI.Attribute("QualityOfLifeExpectation", false, Klei.AI.Attribute.Display.Normal, false, 0f, null, null, null, null));
			this.QualityOfLifeExpectation.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.AirConsumptionRate = base.Add(new Klei.AI.Attribute("AirConsumptionRate", false, Klei.AI.Attribute.Display.Normal, false, 0f, null, null, null, null));
			this.AirConsumptionRate.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			this.MaxUnderwaterTravelCost = base.Add(new Klei.AI.Attribute("MaxUnderwaterTravelCost", false, Klei.AI.Attribute.Display.Normal, false, 0f, null, null, null, null));
			this.ToiletEfficiency = base.Add(new Klei.AI.Attribute("ToiletEfficiency", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, null));
			this.ToiletEfficiency.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			this.Sneezyness = base.Add(new Klei.AI.Attribute("Sneezyness", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, null));
			this.TransitTubeTravelSpeed = base.Add(new Klei.AI.Attribute("TransitTubeTravelSpeed", false, Klei.AI.Attribute.Display.Never, false, 0f, null, null, null, null));
			this.TransitTubeTravelSpeed.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None));
			this.DiseaseCureSpeed = base.Add(new Klei.AI.Attribute("DiseaseCureSpeed", false, Klei.AI.Attribute.Display.Normal, false, 0f, null, null, null, null));
			this.DiseaseCureSpeed.BaseValue = 1f;
			this.DiseaseCureSpeed.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			this.DoctoredLevel = base.Add(new Klei.AI.Attribute("DoctoredLevel", false, Klei.AI.Attribute.Display.Never, false, 0f, null, null, null, null));
			this.CarryAmount = base.Add(new Klei.AI.Attribute("CarryAmount", false, Klei.AI.Attribute.Display.Details, false, 0f, null, null, null, null));
			this.CarryAmount.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None));
			this.QualityOfLife = base.Add(new Klei.AI.Attribute("QualityOfLife", false, Klei.AI.Attribute.Display.Details, false, 0f, "ui_icon_qualityoflife", "attribute_qualityoflife", "mod_morale", null));
			this.QualityOfLife.SetFormatter(new QualityOfLifeAttributeFormatter());
			this.GermResistance = base.Add(new Klei.AI.Attribute("GermResistance", false, Klei.AI.Attribute.Display.Details, false, 0f, "ui_icon_immunelevel", "attribute_immunelevel", "mod_germresistance", null));
			this.GermResistance.SetFormatter(new GermResistanceAttributeFormatter());
			this.LifeSupport = base.Add(new Klei.AI.Attribute("LifeSupport", true, Klei.AI.Attribute.Display.Never, false, 0f, null, null, null, null));
			this.LifeSupport.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
			this.Toggle = base.Add(new Klei.AI.Attribute("Toggle", true, Klei.AI.Attribute.Display.Never, false, 0f, null, null, null, null));
			this.Toggle.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
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

		public Klei.AI.Attribute Ranching;

		public Klei.AI.Attribute LifeSupport;

		public Klei.AI.Attribute Toggle;

		public Klei.AI.Attribute PowerTinker;

		public Klei.AI.Attribute FarmTinker;

		public Klei.AI.Attribute SpaceNavigation;

		public Klei.AI.Attribute Immunity;

		public Klei.AI.Attribute GermResistance;

		public Klei.AI.Attribute Insulation;

		public Klei.AI.Attribute Luminescence;

		public Klei.AI.Attribute ThermalConductivityBarrier;

		public Klei.AI.Attribute Decor;

		public Klei.AI.Attribute FoodQuality;

		public Klei.AI.Attribute ScaldingThreshold;

		public Klei.AI.Attribute ScoldingThreshold;

		public Klei.AI.Attribute GeneratorOutput;

		public Klei.AI.Attribute MachinerySpeed;

		public Klei.AI.Attribute RadiationResistance;

		public Klei.AI.Attribute RadiationRecovery;

		public Klei.AI.Attribute DecorExpectation;

		public Klei.AI.Attribute FoodExpectation;

		public Klei.AI.Attribute RoomTemperaturePreference;

		public Klei.AI.Attribute QualityOfLifeExpectation;

		public Klei.AI.Attribute AirConsumptionRate;

		public Klei.AI.Attribute MaxUnderwaterTravelCost;

		public Klei.AI.Attribute ToiletEfficiency;

		public Klei.AI.Attribute Sneezyness;

		public Klei.AI.Attribute TransitTubeTravelSpeed;

		public Klei.AI.Attribute DiseaseCureSpeed;

		public Klei.AI.Attribute DoctoredLevel;

		public Klei.AI.Attribute CarryAmount;

		public Klei.AI.Attribute QualityOfLife;
	}
}
