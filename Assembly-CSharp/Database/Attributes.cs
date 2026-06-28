using System;
using Klei.AI;

namespace Database
{
	public class Attributes : ResourceSet<Klei.AI.Attribute>
	{
		public Attributes(ResourceSet parent)
			: base("Attributes", parent)
		{
			this.Construction = base.Add(new Klei.AI.Attribute("Construction", true, true, true));
			this.Digging = base.Add(new Klei.AI.Attribute("Digging", true, true, true));
			this.Machinery = base.Add(new Klei.AI.Attribute("Machinery", true, true, true));
			this.Athletics = base.Add(new Klei.AI.Attribute("Athletics", true, true, true));
			this.Learning = base.Add(new Klei.AI.Attribute("Learning", true, true, true));
			this.Cooking = base.Add(new Klei.AI.Attribute("Cooking", true, true, true));
			this.Insulation = base.Add(new Klei.AI.Attribute("Insulation", false, false, true));
			this.Medical = base.Add(new Klei.AI.Attribute("Medical", true, true, true));
			this.Art = base.Add(new Klei.AI.Attribute("Art", true, true, true));
			this.AirConsumptionRate = base.Add(new Klei.AI.Attribute("AirConsumptionRate", false, false, false));
			this.AirConsumptionRate.SetFormatter(new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
			this.Strength = base.Add(new Klei.AI.Attribute("Strength", true, true, true));
			this.DecorExpectation = base.Add(new Klei.AI.Attribute("DecorExpectation", false, false, false));
			this.MaxUnderwaterTravelCost = base.Add(new Klei.AI.Attribute("MaxUnderwaterTravelCost", false, false, false));
			this.ToiletEfficiency = base.Add(new Klei.AI.Attribute("ToiletEfficiency", false, false, false));
		}

		public Klei.AI.Attribute Construction;

		public Klei.AI.Attribute Digging;

		public Klei.AI.Attribute Machinery;

		public Klei.AI.Attribute Athletics;

		public Klei.AI.Attribute Learning;

		public Klei.AI.Attribute Cooking;

		public Klei.AI.Attribute Insulation;

		public Klei.AI.Attribute Medical;

		public Klei.AI.Attribute AirConsumptionRate;

		public Klei.AI.Attribute Strength;

		public Klei.AI.Attribute Art;

		public Klei.AI.Attribute DecorExpectation;

		public Klei.AI.Attribute Decor;

		public Klei.AI.Attribute MaxUnderwaterTravelCost;

		public Klei.AI.Attribute ToiletEfficiency;
	}
}
