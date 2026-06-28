using System;
using STRINGS;

namespace Klei.AI
{
	public class SawCorpsosis : AttributeModifierDisease
	{
		public SawCorpsosis()
			: base("SawCorpsosis", false, new AttributeModifier[]
			{
				new AttributeModifier("StressDelta", 3f, DUPLICANTS.DISEASES.SAWCORPSOSIS.NAME, false, false)
			}, 0.1f, 1200f, null)
		{
		}
	}
}
