using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseMoleConfig
{
	public static GameObject BaseMole(string id, string name, string desc, string traitId, string anim_file, bool is_baby)
	{
		float num = 25f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		EffectorValues none = global::TUNING.BUILDINGS.DECOR.NONE;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, none, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, "DiggerNavGrid", NavType.Floor, 32, 2f, "Meat", 10, true, true, 123.149994f, 673.15f, 73.149994f, 773.15f);
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<DiggerMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DiggerStates.Def(MoleTuning.DEPTH_TO_HIDE), true)
			.Add(new GrowUpStates.Def(), true)
			.Add(new TrappedStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new NestingPoopState.Def((!is_baby) ? SimHashes.Regolith.CreateTag() : Tag.Invalid), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.PopInterruptGroup();
		IdleStates.Def def = new IdleStates.Def();
		def.customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseMoleConfig.CustomIdleAnim);
		ChoreTable.Builder builder2 = builder.Add(def, true);
		EntityTemplates.AddCreatureBrain(gameObject, builder2, GameTags.Creatures.Species.MoleSpecies, null);
		return gameObject;
	}

	public static List<Diet.Info> SimpleOreDiet(List<Tag> elementTags, float caloriesPerKg, float producedConversionRate)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (Tag tag in elementTags)
		{
			list.Add(new Diet.Info(new HashSet<Tag> { tag }, tag, caloriesPerKg, producedConversionRate, null, 0f, true));
		}
		return list;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		int num = Grid.PosToCell(smi.master.gameObject);
		if (Grid.IsSolidCell(num))
		{
			int num2 = global::UnityEngine.Random.Range(0, BaseMoleConfig.SolidIdleAnims.Length);
			return BaseMoleConfig.SolidIdleAnims[num2];
		}
		CreatureCalorieMonitor.Instance smi2 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		return (smi2 == null || !smi2.stomach.IsReadyToPoop()) ? "idle_loop" : "idle_loop_full";
	}

	private static readonly string[] SolidIdleAnims = new string[] { "idle1", "idle2", "idle3", "idle4" };
}
