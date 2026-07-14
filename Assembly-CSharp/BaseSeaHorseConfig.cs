using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseSeaHorseConfig
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float mass = SeaHorseTuning.MASS;
		int num = (is_baby ? 1 : 2);
		EffectorValues tier = DECOR.BONUS.TIER3;
		KAnimFile anim = Assets.GetAnim(is_baby ? anim_file : "seahorse_build_kanim");
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num2 = 1;
		int num3 = num;
		EffectorValues effectorValues = tier;
		float num4 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, mass, anim, text, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, null, num4);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature, false);
		component.AddTag(GameTags.Creatures.Swimmer, false);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SeaHorseTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SeaHorseTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, true, true);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "seahorse_build_kanim", symbol_prefix, FactionManager.FactionID.Prey, base_trait_id, is_baby ? "SwimmerNavGrid" : "SwimmerNavGrid1x2", NavType.Swim, 32, 2f, "FishMeat", 1f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		KAnimFile anim2 = Assets.GetAnim("seahorse_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BaseSeaHorseConfig.GetLandAnim)
			}, true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FlopStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new PoopStates.Def(anim2, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(new MoveToLureStates.Def(), true, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(anim2), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.SeaHorseSpecies, symbol_prefix);
		CritterCondoInteractMontior.Def def = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def.requireCavity = false;
		def.condoPrefabTag = "UnderwaterCritterCondo";
		Tag tag = SimHashes.SlimeMold.CreateTag();
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Pearl.CreateTag());
		Diet diet = new Diet(new List<Diet.Info>
		{
			new Diet.Info(hashSet, tag, BaseSeaHorseConfig.CALORIES_PER_KG_OF_PEARL, BaseSeaHorseConfig.OUTPUT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		}.ToArray());
		CreatureCalorieMonitor.Def def2 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def2.diet = diet;
		def2.minConsumedCaloriesBeforePooping = BaseSeaHorseConfig.CALORIES_PER_KG_OF_PEARL * BaseSeaHorseConfig.MIN_POOP_SIZE_IN_KG;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num5 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["SeaHorse"];
		pickupable.sortOrder = num5;
		if (!is_baby)
		{
			FertilityShearable.Def def3 = gameObject.AddOrGetDef<FertilityShearable.Def>();
			def3.dropMass = 100f;
			def3.milkElement = SimHashes.FishMilk;
			def3.minimumFertility = 75f;
			def3.percentFertilityConsumedPerMilking = 0.5f;
			def3.requiresHappy = true;
			def3.suppressedByElderly = true;
		}
		return gameObject;
	}

	private static string GetLandAnim(FallStates.Instance smi)
	{
		if (smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation())
		{
			return "idle_loop";
		}
		return "flop_loop";
	}

	public const string EMOTION_FILE_NAME = "seahorse_emotes_kanim";

	private const float CLAMS_EATEN_PER_CYCLE = 0.5f;

	private static float KG_PEARL_EATEN_PER_CYCLE = 3.125f;

	private static float CALORIES_PER_KG_OF_PEARL = SeaHorseTuning.STANDARD_CALORIES_PER_CYCLE / BaseSeaHorseConfig.KG_PEARL_EATEN_PER_CYCLE;

	public const float SLIME_PER_CYCLE = 12f;

	public static float OUTPUT_EFFICIENCY = 12f / BaseSeaHorseConfig.KG_PEARL_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;
}
