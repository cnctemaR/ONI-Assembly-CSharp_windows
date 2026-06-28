using System;
using STRINGS;

namespace Klei.AI
{
	public class Lazibonitis : AttributeModifierDisease
	{
		public Lazibonitis()
			: base("Lazibonitis", false, new AttributeModifier[]
			{
				new AttributeModifier("Athletics", -4f, DUPLICANTS.DISEASES.LAZIBONITIS.NAME, false, false),
				new AttributeModifier("Strength", -3f, DUPLICANTS.DISEASES.LAZIBONITIS.NAME, false, false)
			}, 0.1f, 900f, null)
		{
		}
	}
}
