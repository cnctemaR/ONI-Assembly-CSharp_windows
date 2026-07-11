using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BasePacuConfig
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp)
	{
		float num = 200f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		float num2 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, num2);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature);
		component.AddTag(GameTags.Creatures.Swimmer);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PacuTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PacuTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, false);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, base_trait_id, "SwimmerNavGrid", NavType.Swim, 32, 2f, "Meat", 1, false, true, warnLowTemp, warnHighTemp, warnLowTemp - 20f, warnHighTemp + 20f);
		if (is_baby)
		{
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			component2.animWidth = 0.5f;
			component2.animHeight = 0.5f;
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new GrowUpStates.Def(), true)
			.Add(new TrappedStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true);
		FallStates.Def def = new FallStates.Def();
		def.getLandAnim = new Func<FallStates.Instance, string>(BasePacuConfig.GetLandAnim);
		ChoreTable.Builder builder2 = builder.Add(def, true).Add(new DebugGoToStates.Def(), true).Add(new FlopStates.Def(), true)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "lay_egg_pre", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.Add(new MoveToLureStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		CreatureFallMonitor.Def def2 = gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		def2.canSwim = true;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder2, GameTags.Creatures.Species.PacuSpecies, symbol_prefix);
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag> { SimHashes.Algae.CreateTag() }, SimHashes.ToxicSand.CreateTag(), BasePacuConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def3 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def3.diet = diet;
		def3.minPoopSizeInCalories = BasePacuConfig.CALORIES_PER_KG_OF_ORE * BasePacuConfig.MIN_POOP_SIZE_IN_KG;
		SolidConsumerMonitor.Def def4 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def4.diet = diet;
		LureableMonitor.Def def5 = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def5.lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim("pacu_kanim"), symbol_prefix, 0);
		}
		return gameObject;
	}

	private static string GetLandAnim(FallStates.Instance smi)
	{
		if (smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation(true))
		{
			return "idle_loop";
		}
		return "flop_loop";
	}

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_PER_KG_OF_ORE = PacuTuning.STANDARD_CALORIES_PER_CYCLE / BasePacuConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;
}
