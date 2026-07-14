using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Garnish
{
	public KAnim.Build.Symbol GetOverrideSymbol()
	{
		if (!this.overrideAnimName.IsValid)
		{
			return null;
		}
		KAnimFile anim = Assets.GetAnim(this.overrideAnimName);
		if (anim == null)
		{
			return null;
		}
		return anim.GetData().build.GetSymbol(this.overrideSymbolName);
	}

	public EffectInstance Activate(Storage storage, GameObject diner)
	{
		storage.ConsumeIgnoringDisease(this.itemTag, this.consumeRate);
		EffectInstance effectInstance = diner.GetComponent<Effects>().Add(this.effectId, true);
		KAnim.Build.Symbol overrideSymbol = this.GetOverrideSymbol();
		SymbolOverrideController symbolOverrideController;
		if (overrideSymbol != null && diner.TryGetComponent<SymbolOverrideController>(out symbolOverrideController))
		{
			symbolOverrideController.AddSymbolOverride(Garnish.SALT_SYMBOL, overrideSymbol, 0);
			symbolOverrideController.AddSymbolOverride(Garnish.SALT_FG_SYMBOL, overrideSymbol, 0);
		}
		KBatchedAnimController kbatchedAnimController;
		if (diner.TryGetComponent<KBatchedAnimController>(out kbatchedAnimController))
		{
			kbatchedAnimController.SetSymbolTint(Garnish.SALT_PARTICLE_SYMBOL, this.fxTintColor);
		}
		return effectInstance;
	}

	public static void Deactivate(GameObject diner)
	{
		SymbolOverrideController symbolOverrideController;
		if (diner.TryGetComponent<SymbolOverrideController>(out symbolOverrideController))
		{
			symbolOverrideController.RemoveSymbolOverride(Garnish.SALT_SYMBOL, 0);
			symbolOverrideController.RemoveSymbolOverride(Garnish.SALT_FG_SYMBOL, 0);
		}
		KBatchedAnimController kbatchedAnimController;
		if (diner.TryGetComponent<KBatchedAnimController>(out kbatchedAnimController))
		{
			kbatchedAnimController.SetSymbolTint(Garnish.SALT_PARTICLE_SYMBOL, Color.white);
		}
	}

	public static void SetDinerVisibility(KAnimControllerBase controller, bool visible)
	{
		controller.SetSymbolVisiblity(Garnish.SALT_SYMBOL, visible);
		controller.SetSymbolVisiblity(Garnish.SALT_FG_SYMBOL, visible);
	}

	public static Garnish GetActive(Storage storage)
	{
		if (storage == null)
		{
			return null;
		}
		Garnish garnish = null;
		foreach (Garnish garnish2 in Garnish.All)
		{
			if (storage.GetMassAvailable(garnish2.itemTag) >= garnish2.consumeRate && (garnish == null || garnish2.priority > garnish.priority))
			{
				garnish = garnish2;
			}
		}
		return garnish;
	}

	public static bool HasAny(Storage storage)
	{
		return Garnish.GetActive(storage) != null;
	}

	public Tag itemTag;

	public string effectId;

	public float consumeRate;

	public int priority;

	public Descriptor descriptor;

	private HashedString overrideAnimName;

	private KAnimHashedString overrideSymbolName;

	private Color fxTintColor = Color.white;

	private static readonly HashedString SALT_SYMBOL = "saltshaker";

	private static readonly HashedString SALT_FG_SYMBOL = "saltshaker_fg";

	private static readonly KAnimHashedString SALT_PARTICLE_SYMBOL = new KAnimHashedString("salt_particle");

	public static readonly List<Garnish> All = new List<Garnish>
	{
		new Garnish
		{
			itemTag = TableSaltConfig.TAG,
			effectId = "MessTableSalt",
			consumeRate = TableSaltTuning.CONSUMABLE_RATE,
			priority = 0,
			descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), Descriptor.DescriptorType.Effect, false)
		},
		new Garnish
		{
			itemTag = CaviarConfig.TAG,
			effectId = "MessCaviar",
			consumeRate = CaviarTuning.CONSUMABLE_RATE,
			priority = 10,
			descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_CAVIAR, CaviarTuning.MORALE_MODIFIER, CaviarTuning.STRESS_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_CAVIAR, CaviarTuning.MORALE_MODIFIER, CaviarTuning.STRESS_MODIFIER), Descriptor.DescriptorType.Effect, false),
			overrideAnimName = "caviarshaker_kanim",
			overrideSymbolName = "object",
			fxTintColor = Color.black
		}
	};
}
