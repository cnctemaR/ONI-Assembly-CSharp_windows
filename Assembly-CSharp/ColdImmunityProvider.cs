using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class ColdImmunityProvider : EffectImmunityProviderStation<ColdImmunityProvider.Instance>
{
	public const string PROVIDED_IMMUNITY_EFFECT_NAME = "WarmTouch";

	public new class Def : EffectImmunityProviderStation<ColdImmunityProvider.Instance>.Def, IGameObjectEffectDescriptor
	{
		public override string[] DefaultAnims()
		{
			return new string[] { "warmup_pre", "warmup_loop", "warmup_pst" };
		}

		public override string DefaultAnimFileName()
		{
			return "anim_warmup_kanim";
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + "WarmTouch".ToUpper() + ".PROVIDERS_NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + "WarmTouch".ToUpper() + ".PROVIDERS_TOOLTIP"), Descriptor.DescriptorType.Effect, false)
			};
		}
	}

	public new class Instance : EffectImmunityProviderStation<ColdImmunityProvider.Instance>.BaseInstance
	{
		public Instance(IStateMachineTarget master, ColdImmunityProvider.Def def)
			: base(master, def)
		{
		}

		protected override void ApplyImmunityEffect(Effects target)
		{
			target.Add("WarmTouch", true);
		}
	}
}
