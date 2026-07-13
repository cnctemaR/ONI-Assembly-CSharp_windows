using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BasePacuConfig
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float mass = PacuTuning.MASS;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num = 1;
		int num2 = 1;
		EffectorValues effectorValues = tier;
		float num3 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, mass, anim, text, sceneLayer, num, num2, effectorValues, default(EffectorValues), SimHashes.Creature, null, num3);
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
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "pacu_build_kanim", symbol_prefix, FactionManager.FactionID.Prey, base_trait_id, "SwimmerNavGrid", NavType.Swim, 32, 2f, "FishMeat", 1f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
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
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("pacu_emotes_kanim")), true, -1)
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
		CritterCondoInteractMontior.Def def2 = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def2.requireCavity = false;
		def2.condoPrefabTag = "UnderwaterCritterCondo";
		Tag tag = SimHashes.ToxicSand.CreateTag();
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Algae.CreateTag());
		List<Diet.Info> list = new List<Diet.Info>
		{
			new Diet.Info(hashSet, tag, BasePacuConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		};
		if (DlcManager.GetActiveDLCIds().Contains("DLC4_ID"))
		{
			HashSet<Tag> hashSet2 = new HashSet<Tag>();
			hashSet2.Add(KelpConfig.ID);
			HashSet<Tag> hashSet3 = new HashSet<Tag>();
			hashSet3.Add("KelpPlant");
			list.AddRange(new List<Diet.Info>
			{
				new Diet.Info(hashSet2, tag, BasePacuConfig.CALORIES_PER_KG_OF_KELP, BasePacuConfig.KELP_TO_PRODUCT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null),
				new Diet.Info(hashSet3, tag, BasePacuConfig.CALORIES_PER_GROWTH_EATEN, BasePacuConfig.GROWTH_TO_PRODUCT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatPlantDirectly, false, null)
			});
		}
		list.AddRange(BasePacuConfig.SeedDiet(tag, PacuTuning.STANDARD_CALORIES_PER_CYCLE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL));
		Diet diet = new Diet(list.ToArray());
		CreatureCalorieMonitor.Def def3 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def3.diet = diet;
		def3.minConsumedCaloriesBeforePooping = BasePacuConfig.CALORIES_PER_KG_OF_ORE * BasePacuConfig.MIN_POOP_SIZE_IN_KG;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num4 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Pacu"];
		pickupable.sortOrder = num4;
		return gameObject;
	}

	public static List<Diet.Info> SeedDiet(Tag poopTag, float caloriesPerSeed, float producedConversionRate)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<PlantableSeed>())
		{
			GameObject prefab = Assets.GetPrefab(gameObject.GetComponent<PlantableSeed>().PlantID);
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (!prefab.HasTag(GameTags.DeprecatedContent) && !component.HasTag("KelpPlantSeed"))
			{
				SeedProducer component2 = prefab.GetComponent<SeedProducer>();
				if (component2 == null || component2.seedInfo.productionType == SeedProducer.ProductionType.Harvest || component2.seedInfo.productionType == SeedProducer.ProductionType.Crop || component2.seedInfo.productionType == SeedProducer.ProductionType.HarvestOnly)
				{
					list.Add(new Diet.Info(new HashSet<Tag>
					{
						new Tag(component.GetComponent<KPrefabID>().PrefabID())
					}, poopTag, caloriesPerSeed, producedConversionRate, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null));
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

	public const float UNITS_OF_ALGAE_FROM_ONE_UNIT_OF_KELP = 2.6666667f;

	public static float KG_KELP_EATEN_PER_CYCLE = 20f;

	public static float KELP_TO_PRODUCT_EFFICIENCY = global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL;

	private static float CALORIES_PER_KG_OF_KELP = PacuTuning.STANDARD_CALORIES_PER_CYCLE / BasePacuConfig.KG_KELP_EATEN_PER_CYCLE;

	private static float KELP_PLANTS_PER_PACU = BasePacuConfig.KG_KELP_EATEN_PER_CYCLE / 10f;

	private static float KELP_GROWTH_EATEN_PER_CYCLE = 0.2f * BasePacuConfig.KELP_PLANTS_PER_PACU;

	private static float CALORIES_PER_GROWTH_EATEN = PacuTuning.STANDARD_CALORIES_PER_CYCLE / (BasePacuConfig.KELP_GROWTH_EATEN_PER_CYCLE * 5f);

	private static float GROWTH_TO_PRODUCT_EFFICIENCY = BasePacuConfig.KELP_TO_PRODUCT_EFFICIENCY * 10f;

	private static float MIN_POOP_SIZE_IN_KG = 25f;
}
