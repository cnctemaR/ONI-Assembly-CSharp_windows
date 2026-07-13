using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseMooConfig
{
	public static GameObject BaseMoo(string id, string name, string desc, string traitId, string anim_file, List<BeckoningMonitor.SongChance> initialSongChances, bool is_baby, string symbol_override_prefix)
	{
		float num = 50f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 2, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "gassy_moo_build_kanim", symbol_override_prefix, FactionManager.FactionID.Prey, traitId, "FlyerNavGrid2x2", NavType.Hover, 32, 2f, "Meat", 10f, true, true, 223.15f, 323.15f, 73.149994f, 473.15f);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_override_prefix, null, 0);
		}
		if (!is_baby)
		{
			KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
			kboxCollider2D.offset = new Vector2f(0f, kboxCollider2D.offset.y);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Moo"];
		pickupable.sortOrder = num2;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Flyer, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		BeckoningMonitor.Def def = gameObject.AddOrGetDef<BeckoningMonitor.Def>();
		def.initialSongWeights = initialSongChances;
		def.caloriesPerCycle = MooTuning.WELLFED_CALORIES_PER_CYCLE;
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			SimHashes.BleachStone.CreateTag(),
			GameTags.Creatures.FlyersLure
		};
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		gameObject.AddOrGetDef<RanchableMonitor.Def>();
		gameObject.AddOrGetDef<FixedCapturableMonitor.Def>();
		MilkProductionMonitor.Def def2 = gameObject.AddOrGetDef<MilkProductionMonitor.Def>();
		def2.CaloriesPerCycle = MooTuning.WELLFED_CALORIES_PER_CYCLE;
		def2.Capacity = MooTuning.MILK_CAPACITY;
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new TrappedStates.Def(), true, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new BeckonFromSpaceStates.Def(), true, -1)
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def
			{
				WaitCellOffset = 2
			}, true, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = false,
				drinkCellOffsetGetFn = new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_GassyMoo)
			}, true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.TOOLTIP), true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CritterCondoStates.Def
			{
				working_anim = "cc_working_moo"
			}, !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("gassy_moo_emotes_kanim")), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseMooConfig.CustomIdleAnim)
			}, true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.MooSpecies, symbol_override_prefix);
		gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>().condoPrefabTag = "AirBorneCritterCondo";
		return gameObject;
	}

	public static void SetupBaseDiet(GameObject prefab, Tag producedTag)
	{
		Diet diet = BaseMooConfig.ExpandDiet(null, prefab, "GasGrass".ToTag(), producedTag, MooTuning.CALORIES_PER_DAY_OF_PLANT_EATEN, MooTuning.KG_POOP_PER_DAY_OF_PLANT, Diet.Info.FoodType.EatPlantDirectly, MooTuning.MIN_POOP_SIZE_IN_KG);
		diet = BaseMooConfig.ExpandDiet(diet, prefab, "PlantFiber".ToTag(), producedTag, MooTuning.CALORIES_PER_DAY_OF_SOLID_EATEN, MooTuning.POOP_KG_COVERSION_RATE_FOR_SOLID_DIET, Diet.Info.FoodType.EatSolid, MooTuning.MIN_POOP_SIZE_IN_KG);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = MooTuning.MIN_POOP_SIZE_IN_CALORIES;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
	}

	public static Diet ExpandDiet(Diet diet, GameObject prefab, Tag consumed_tag, Tag producedTag, float caloriesPerKg, float producedConversionRate, Diet.Info.FoodType foodType, float minPoopSizeInKg)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(consumed_tag);
		Diet.Info[] array = ((diet != null) ? new Diet.Info[diet.infos.Length + 1] : new Diet.Info[1]);
		if (diet != null)
		{
			for (int i = 0; i < diet.infos.Length; i++)
			{
				array[i] = diet.infos[i];
			}
		}
		array[array.Length - 1] = new Diet.Info(hashSet, producedTag, caloriesPerKg, producedConversionRate, null, 0f, false, foodType, false, null);
		return new Diet(array);
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		CreatureCalorieMonitor.Instance smi2 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		return (smi2 != null && smi2.stomach.IsReadyToPoop()) ? "idle_loop_full" : "idle_loop";
	}

	public static void OnSpawn(GameObject inst)
	{
		Navigator component = inst.GetComponent<Navigator>();
		component.transitionDriver.overrideLayers.Add(new FullPuftTransitionLayer(component));
	}
}
