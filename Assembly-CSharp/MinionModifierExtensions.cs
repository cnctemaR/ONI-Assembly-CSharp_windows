using System;
using Klei.AI;

public static class MinionModifierExtensions
{
	public static EffectInstance GetMentalBreakEffect(this Effects effects)
	{
		return effects.Get(Db.Get().mentalBreakEffect);
	}
}
