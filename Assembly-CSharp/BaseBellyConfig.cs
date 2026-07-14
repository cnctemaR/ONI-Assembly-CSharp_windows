using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseBellyConfig
{
	public static GameObject BaseBelly(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 400f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(is_baby ? anim_file : "ice_belly_build_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 2, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2f(0f, kboxCollider2D.offset.y);
		gameObject.GetComponent<KBatchedAnimController>().Offset = new Vector3(0f, 0f, 0f);
		string text = "WalkerNavGrid2x2";
		if (is_baby)
		{
			text = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(true, gameObject, anim_file, is_baby ? null : "ice_belly_build_kanim", symbolOverridePrefix, FactionManager.FactionID.Pest, traitId, text, NavType.Floor, 32, 2f, "Meat", 14f, true, false, 303.15f, 343.15f, 173.15f, 373.15f);
		gameObject.AddOrGet<Navigator>();
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["IceBelly"];
		pickupable.sortOrder = num2;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<WorldSpawnableMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		bool flag = !is_baby;
		KAnimFile anim = Assets.GetAnim("ice_belly_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", global::STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, global::STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), flag, -1)
			.Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), flag, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def
			{
				WaitCellOffset = 2
			}, !is_baby, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", global::STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, global::STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), flag, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = false,
				drinkCellOffsetGetFn = (is_baby ? new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_CritterOneByOne) : new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_TwoByTwo))
			}, true, -1)
			.Add(new PoopStates.Def(anim, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(anim), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.BellySpecies, symbolOverridePrefix);
		gameObject.AddOrGet<OccupyArea>().updateWithFacing = !is_baby;
		return gameObject;
	}

	public static Diet.Info BasicDiet(Tag foodTag, Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		return new Diet.Info(new HashSet<Tag> { foodTag }, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatPlantDirectly, true, null);
	}

	public static List<Diet.Info> StandardDiets()
	{
		return new List<Diet.Info>
		{
			BaseBellyConfig.BasicDiet("CarrotPlant", "IceBellyPoop", BellyTuning.CALORIES_PER_UNIT_EATEN / BellyTuning.CONSUMABLE_PLANT_MATURITY_LEVELS, 67.474f / BellyTuning.CONSUMABLE_PLANT_MATURITY_LEVELS, BellyTuning.GERM_ID_EMMITED_ON_POOP, 1000f),
			new Diet.Info(new HashSet<Tag> { CarrotConfig.ID }, "IceBellyPoop", BellyTuning.CALORIES_PER_UNIT_EATEN / 1f, 67.474f, BellyTuning.GERM_ID_EMMITED_ON_POOP, 1000f, false, Diet.Info.FoodType.EatSolid, true, null),
			new Diet.Info(new HashSet<Tag> { "FriesCarrot" }, "IceBellyPoop", FOOD.FOOD_TYPES.FRIES_CARROT.CaloriesPerUnit / 0.75928f, 120.01f, BellyTuning.GERM_ID_EMMITED_ON_POOP, 1000f, false, Diet.Info.FoodType.EatSolid, true, null),
			BaseBellyConfig.BasicDiet("BeanPlant", "IceBellyPoop", BellyTuning.CALORIES_PER_UNIT_EATEN / BellyTuning.CONSUMABLE_PLANT_MATURITY_LEVELS / 0.744f, 67.474f / BellyTuning.CONSUMABLE_PLANT_MATURITY_LEVELS / 0.744f, BellyTuning.GERM_ID_EMMITED_ON_POOP, 1000f),
			new Diet.Info(new HashSet<Tag> { "BeanPlantSeed" }, "IceBellyPoop", 1100000f, 18.570995f, BellyTuning.GERM_ID_EMMITED_ON_POOP, 1000f, false, Diet.Info.FoodType.EatSolid, true, null)
		};
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float referenceCaloriesPerKg, float minPoopSizeInKg)
	{
		Diet diet = new Diet(diet_infos.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = referenceCaloriesPerKg * minPoopSizeInKg;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}

	public const string EMOTION_FILE_NAME = "ice_belly_emotes_kanim";
}
