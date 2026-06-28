using System;

namespace Klei.AI.DiseaseGrowthRules
{
	public class StateGrowthRule : GrowthRule
	{
		public StateGrowthRule(Element.State state)
		{
			this.state = state;
		}

		public override bool Test(Element e)
		{
			return e.IsState(this.state);
		}

		public override string Name()
		{
			return Element.GetStateString(this.state);
		}

		public Element.State state;
	}
}
