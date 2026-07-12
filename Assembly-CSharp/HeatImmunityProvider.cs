using System;
using Klei.AI;

public class HeatImmunityProvider : EffectImmunityProviderStation<HeatImmunityProvider.Instance>
{
	public const string PROVIDED_IMMUNITY_EFFECT_NAME = "RefreshingTouch";

	public new class Def : EffectImmunityProviderStation<HeatImmunityProvider.Instance>.Def
	{
	}

	public new class Instance : EffectImmunityProviderStation<HeatImmunityProvider.Instance>.BaseInstance
	{
		public Instance(IStateMachineTarget master, HeatImmunityProvider.Def def)
			: base(master, def)
		{
		}

		protected override void ApplyImmunityEffect(Effects target)
		{
			target.Add("RefreshingTouch", true);
		}
	}
}
