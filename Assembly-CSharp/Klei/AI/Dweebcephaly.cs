using System;
using STRINGS;

namespace Klei.AI
{
	public class Dweebcephaly : AttributeModifierDisease
	{
		public Dweebcephaly()
			: base("Dweebcephaly", false, new AttributeModifier[]
			{
				new AttributeModifier("Learning", -2f, DUPLICANTS.DISEASES.DWEEBCEPHALY.NAME, false)
			}, 0.005f, 900f, new Disease.EffectProbabilityDelta[]
			{
				new Disease.EffectProbabilityDelta
				{
					effectID = "DirtyHands",
					probabilityDelta = 0.005f
				}
			})
		{
		}
	}
}
