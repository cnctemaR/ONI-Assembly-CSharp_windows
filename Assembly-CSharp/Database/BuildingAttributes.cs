using System;
using Klei.AI;

namespace Database
{
	public class BuildingAttributes : ResourceSet<Klei.AI.Attribute>
	{
		public BuildingAttributes(ResourceSet parent)
			: base("BuildingAttributes", parent)
		{
			this.Decor = base.Add(new Klei.AI.Attribute("Decor", true, true, false));
			this.DecorRadius = base.Add(new Klei.AI.Attribute("DecorRadius", true, true, false));
			this.Hygiene = base.Add(new Klei.AI.Attribute("Hygiene", true, true, false));
			this.Comfort = base.Add(new Klei.AI.Attribute("Comfort", true, true, false));
		}

		public Klei.AI.Attribute Decor;

		public Klei.AI.Attribute DecorRadius;

		public Klei.AI.Attribute Hygiene;

		public Klei.AI.Attribute Comfort;
	}
}
