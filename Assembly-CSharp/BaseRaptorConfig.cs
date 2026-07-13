using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseRaptorConfig
{
	public static GameObject BaseRaptor(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
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
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "raptor_build_kanim", null, FactionManager.FactionID.Predator, traitId, text, NavType.Floor, 32, 2f, "DinosaurMeat", 5f, true, false, 223.15f, 288.15f, 173.15f, 373.15f);
		gameObject.AddOrGet<Navigator>();
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		gameObject.AddOrGet<Pickupable>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<CritterRoarMonitor.Def>().Initialize(RaptorTuning.ROARS_PER_CYCLE, RaptorTuning.ROAR_COOLDOWN);
		gameObject.AddOrGetDef<WorldSpawnableMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
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
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), false, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = false,
				drinkCellOffsetGetFn = (is_baby ? new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_CritterOneByOne) : new DrinkMilkStates.Def.DrinkCellOffsetGetFn(DrinkMilkStates.Def.DrinkCellOffsetGet_TwoByTwo))
			}, true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def
			{
				entersBuilding = false
			}, !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim("raptor_emotes_kanim")), !is_baby, -1)
			.Add(new CritterRoarStates.Def(), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.RaptorSpecies, symbolOverridePrefix);
		gameObject.AddOrGet<OccupyArea>().updateWithFacing = !is_baby;
		return gameObject;
	}

	public static List<Diet.Info> StandardDiets()
	{
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(new HashSet<Tag> { "DinosaurMeat", "Meat" }, RaptorTuning.POOP_ELEMENT, RaptorTuning.CALORIES_PER_UNIT_EATEN, RaptorTuning.BASE_PRODUCTION_RATE, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null));
		HashSet<Tag> hashSet = new HashSet<Tag>
		{
			"Hatch", "HatchBaby", "HatchVeggie", "HatchVeggieBaby", "HatchMetal", "HatchMetalBaby", "HatchHard", "HatchHardBaby", "Squirrel", "SquirrelBaby",
			"SquirrelHug", "SquirrelHugBaby", "Mole", "MoleBaby", "MoleDelicacy", "MoleDelicacyBaby", "Oilfloater", "OilfloaterBaby", "OilfloaterDecor", "OilfloaterDecorBaby",
			"OilfloaterHighTemp", "OilfloaterHighTempBaby", "Drecko", "DreckoBaby", "DreckoPlastic", "DreckoPlasticBaby", "StegoBaby", "AlgaeStegoBaby", "Chameleon", "ChameleonBaby"
		};
		if (DlcManager.IsContentSubscribed("EXPANSION1_ID"))
		{
			hashSet.Add("DivergentWorm");
			hashSet.Add("DivergentWormBaby");
			hashSet.Add("DivergentBeetle");
			hashSet.Add("DivergentBeetleBaby");
			hashSet.Add("Staterpillar");
			hashSet.Add("StaterpillarBaby");
			hashSet.Add("StaterpillarGas");
			hashSet.Add("StaterpillarGasBaby");
			hashSet.Add("StaterpillarLiquid");
			hashSet.Add("StaterpillarLiquidBaby");
		}
		if (DlcManager.IsContentSubscribed("DLC2_ID"))
		{
			hashSet.Add("IceBellyBaby");
			hashSet.Add("GoldBellyBaby");
			hashSet.Add("WoodDeer");
			hashSet.Add("WoodDeerBaby");
			hashSet.Add("GlassDeer");
			hashSet.Add("GlassDeerBaby");
		}
		list.Add(new Diet.Info(hashSet, RaptorTuning.POOP_ELEMENT, RaptorTuning.CALORIES_PER_UNIT_EATEN, RaptorTuning.PREY_PRODUCTION_RATE, null, 0f, false, Diet.Info.FoodType.EatButcheredPrey, false, null));
		return list;
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos)
	{
		Diet diet = new Diet(diet_infos.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = RaptorTuning.CALORIES_PER_UNIT_EATEN * 0.1f;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.possibleEatPositionOffsets = new Vector3[]
		{
			Vector2.left,
			Vector2.right
		};
		def2.navigatorSize = new Vector2(2f, 2f);
		def2.diet = diet;
		return prefab;
	}
}
