using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseStegoConfig
{
	public static GameObject BaseStego(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 400f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 2, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2f(0f, kboxCollider2D.offset.y);
		gameObject.GetComponent<KBatchedAnimController>().Offset = new Vector3(0f, 0f, 0f);
		string text = "WalkerNavGrid2x2";
		if (is_baby)
		{
			text = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, text, NavType.Floor, 32, 1.5f, "DinosaurMeat", 12f, true, false, 293.15f, 343.15f, 173.15f, 373.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Stego"];
		pickupable.sortOrder = num2;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		if (!is_baby)
		{
			StompMonitor.Def def = gameObject.AddOrGetDef<StompMonitor.Def>();
			def.Cooldown = 60f;
			def.radius = 10;
		}
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), true, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), is_baby, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def
			{
				WaitCellOffset = 2
			}, !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new StompStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = false,
				drinkCellOffsetGetFn = (is_baby ? new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_CritterOneByOne) : new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_TwoByTwo))
			}, true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.StegoSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static List<Diet.Info> StandardDiets()
	{
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(new HashSet<Tag> { VineFruitConfig.ID }, StegoTuning.POOP_ELEMENT, StegoTuning.CALORIES_PER_KG_OF_ORE, StegoTuning.PEAT_PRODUCED_PER_CYCLE / StegoTuning.VINE_FOOD_PER_CYCLE, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null));
		float num = FOOD.FOOD_TYPES.PRICKLEFRUIT.CaloriesPerUnit / FOOD.FOOD_TYPES.VINEFRUIT.CaloriesPerUnit;
		list.Add(new Diet.Info(new HashSet<Tag> { PrickleFruitConfig.ID }, StegoTuning.POOP_ELEMENT, StegoTuning.CALORIES_PER_KG_OF_ORE * num, StegoTuning.PEAT_PRODUCED_PER_CYCLE / (StegoTuning.VINE_FOOD_PER_CYCLE / num), null, 0f, false, Diet.Info.FoodType.EatSolid, false, null));
		if (DlcManager.IsExpansion1Active())
		{
			float num2 = FOOD.FOOD_TYPES.SWAMPFRUIT.CaloriesPerUnit / FOOD.FOOD_TYPES.VINEFRUIT.CaloriesPerUnit;
			float num3 = 1.5f;
			list.Add(new Diet.Info(new HashSet<Tag> { SwampFruitConfig.ID }, StegoTuning.POOP_ELEMENT, StegoTuning.CALORIES_PER_KG_OF_ORE * num2, StegoTuning.PEAT_PRODUCED_PER_CYCLE / (StegoTuning.VINE_FOOD_PER_CYCLE / num2) * num3, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null));
		}
		return list;
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> dietInfos, float referenceCaloriesPerKg, float minPoopSizeInKg)
	{
		Diet diet = new Diet(dietInfos.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = referenceCaloriesPerKg * minPoopSizeInKg;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}
}
