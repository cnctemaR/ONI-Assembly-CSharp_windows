using System;

namespace Klei.AI.DiseaseGrowthRules
{
	public class ElementGrowthRule : GrowthRule
	{
		public ElementGrowthRule(SimHashes element)
		{
			this.element = element;
		}

		public override bool Test(Element e)
		{
			return e.id == this.element;
		}

		public override string Name()
		{
			return ElementLoader.FindElementByHash(this.element).name;
		}

		public SimHashes element;
	}
}
