using System;
using Database;
using Klei.AI;

public class EntityModifierSet : ModifierSet
{
	public override void Initialize()
	{
		base.Initialize();
		this.DuplicantStatusItems = new DuplicantStatusItems(this.Root);
		this.ChoreGroups = new ChoreGroups(this.Root);
		base.LoadTraits();
		this.LoadEffectCallbacks();
	}

	private void LoadEffectCallbacks()
	{
		this.effects.Get("UncomfortableSleep").OnAddRemove += this.OnUncomfortableSleep;
	}

	private void OnUncomfortableSleep(Effects effects, Effect effect, bool is_add)
	{
		if (!is_add)
		{
			effects.GetAmounts().Get("Stamina").SetValue(90f);
		}
	}

	public DuplicantStatusItems DuplicantStatusItems;

	public ChoreGroups ChoreGroups;
}
