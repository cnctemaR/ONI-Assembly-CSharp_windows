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
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, (warnLowTemp + warnHighTemp) / 2f);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature, false);
		component.AddTag(GameTags.Creatures.Swimmer, false);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PacuTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PacuTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, false, true);
		gameObject.AddComponent<Movable>();
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, base_trait_id, "SwimmerNavGrid", NavType.Swim, 32, 2f, "FishMeat", 1, false, false, warnLowTemp, warnHighTemp, warnLowTemp - 20f, warnHighTemp + 20f);
		if (is_baby)
		{
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			component2.animWidth = 0.5f;
			component2.animHeight = 0.5f;
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BasePacuConfig.GetLandAnim)
			}, true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FlopStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "lay_egg_pre", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		CreatureFallMonitor.Def def = gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		def.canSwim = true;
		def.checkHead = false;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.PacuSpecies, symbol_prefix);
		gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>().useunderWaterCondos = true;
		Tag tag = SimHashes.ToxicSand.CreateTag();
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Algae.CreateTag());
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(hashSet, tag, BasePacuConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, false, false));
		list.AddRange(BasePacuConfig.SeedDiet(tag, PacuTuning.STANDARD_CALORIES_PER_CYCLE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL));
		Diet diet = new Diet(list.ToArray());
		CreatureCalorieMonitor.Def def2 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def2.diet = diet;
		def2.minPoopSizeInCalories = BasePacuConfig.CALORIES_PER_KG_OF_ORE * BasePacuConfig.MIN_POOP_SIZE_IN_KG;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			GameTags.Creatures.FishTrapLure,
			GameTags.Creatures.FlyersLure
		};
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Pacu"];
		pickupable.sortOrder = num2;
		return gameObject;
	}

	public static List<Diet.Info> SeedDiet(Tag poopTag, float caloriesPerSeed, float producedConversionRate)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (GameObject gameObject in Assets.GetPrefabsWithTag(GameTags.Seed))
		{
			GameObject prefab = Assets.GetPrefab(gameObject.GetComponent<PlantableSeed>().PlantID);
			if (!prefab.HasTag(GameTags.DeprecatedContent))
			{
				SeedProducer component = prefab.GetComponent<SeedProducer>();
				if (component == null || component.seedInfo.productionType == SeedProducer.ProductionType.Harvest)
				{
					list.Add(new Diet.Info(new HashSet<Tag>
					{
						new Tag(gameObject.GetComponent<KPrefabID>().PrefabID())
					}, poopTag, caloriesPerSeed, producedConversionRate, null, 0f, false, false));
				}
			}
		}
		return list;
	}

	private static string GetLandAnim(FallStates.Instance smi)
	{
		if (smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation())
		{
			return "idle_loop";
		}
		return "flop_loop";
	}

	private static float KG_ORE_EATEN_PER_CYCLE = 7.5f;

	private static float CALORIES_PER_KG_OF_ORE = PacuTuning.STANDARD_CALORIES_PER_CYCLE / BasePacuConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;
}
