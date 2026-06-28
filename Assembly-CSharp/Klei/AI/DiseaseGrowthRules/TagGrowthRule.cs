using System;

namespace Klei.AI.DiseaseGrowthRules
{
	public class TagGrowthRule : GrowthRule
	{
		public TagGrowthRule(Tag tag)
		{
			this.tag = tag;
		}

		public override bool Test(Element e)
		{
			return e.HasTag(this.tag);
		}

		public override string Name()
		{
			return this.tag.ProperName();
		}

		public Tag tag;
	}
}
