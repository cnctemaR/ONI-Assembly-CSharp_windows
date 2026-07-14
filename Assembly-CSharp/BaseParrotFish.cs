using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BaseParrotFish
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float mass = ParrotFishTuning.MASS;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim(is_baby ? anim_file : "fish_bioluminescent_build_kanim");
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
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, ParrotFishTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -ParrotFishTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, true, true);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "fish_bioluminescent_build_kanim", symbol_prefix, FactionManager.FactionID.Prey, base_trait_id, "SwimmerNavGrid", NavType.Swim, 32, 2f, "FishMeat", 1f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.Color = LIGHT2D.PARROTFISH_COLOR;
		light2D.overlayColour = LIGHT2D.PARROTFISH_OVERLAYCOLOR;
		light2D.Range = 5f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.PARROTFISH_DIRECTION;
		light2D.Offset = LIGHT2D.PARROTFISH_OFFSET;
		light2D.shape = global::LightShape.Circle;
		light2D.drawOverlay = true;
		light2D.Lux = 5000;
		light2D.IntensityAnimation = 0.2f;
		gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
		gameObject.AddOrGetDef<CreatureLightToggleController.Def>();
		KAnimFile anim2 = Assets.GetAnim("fish_bioluminescent_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BaseParrotFish.GetLandAnim)
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
			.Add(new SurfaceAirConsumerStates.Def
			{
				effectId = "SurfaceAirConsumed",
				consumptionRate = 3f,
				consumeDuration = 6f
			}, true, -1)
			.Add(new CritterEmoteStates.Def(anim2), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		CreatureFallMonitor.Def def = gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		def.canSwim = true;
		def.checkHead = false;
		gameObject.AddOrGetDef<FlopMonitor.Def>();
		gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.ParrotFishSpecies, symbol_prefix);
		CritterCondoInteractMontior.Def def2 = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def2.requireCavity = false;
		def2.condoPrefabTag = "UnderwaterCritterCondo";
		SurfaceAirConsumerMonitor.Def def3 = gameObject.AddOrGetDef<SurfaceAirConsumerMonitor.Def>();
		def3.element = SimHashes.Oxygen;
		def3.minimumMassThreshold = 2f;
		def3.cooldown = 600f;
		Effect effect = new Effect("SurfaceAirConsumed", global::STRINGS.CREATURES.MODIFIERS.SURFACEAIRCONSUMED.NAME, global::STRINGS.CREATURES.MODIFIERS.SURFACEAIRCONSUMED.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 2f, global::STRINGS.CREATURES.MODIFIERS.SURFACEAIRCONSUMED.NAME, false, false, true));
		Db.Get().effects.Add(effect);
		Tag tag = SimHashes.Lime.CreateTag();
		new HashSet<Tag>();
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Phosphorite.CreateTag());
		HashSet<Tag> hashSet2 = new HashSet<Tag>();
		hashSet2.Add("PlanktonCoral");
		Diet diet = new Diet(new List<Diet.Info>
		{
			new Diet.Info(hashSet, tag, BaseParrotFish.CALORIES_PER_KG_OF_CORAL_PICKUPABLE, BaseParrotFish.CORAL_PICKUPABLE_TO_PRODUCT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null),
			new Diet.Info(hashSet2, tag, BaseParrotFish.CALORIES_PER_GROWTH_EATEN, BaseParrotFish.GROWTH_TO_PRODUCT_EFFICIENCY, null, 0f, false, Diet.Info.FoodType.EatPlantDirectly, false, null)
		}.ToArray());
		CreatureCalorieMonitor.Def def4 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def4.diet = diet;
		def4.minConsumedCaloriesBeforePooping = BaseParrotFish.CALORIES_PER_KG_OF_ORE * BaseParrotFish.MIN_POOP_SIZE_IN_KG;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num4 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["ParrotFish"];
		pickupable.sortOrder = num4;
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

	public const string EMOTION_FILE_NAME = "fish_bioluminescent_emotes_kanim";

	public static float KG_CORAL_PICKUPABLE_EATEN_PER_CYCLE = 10f;

	private static float CALORIES_PER_KG_OF_ORE = ParrotFishTuning.STANDARD_CALORIES_PER_CYCLE / BaseParrotFish.KG_CORAL_PICKUPABLE_EATEN_PER_CYCLE;

	public static float CORAL_PICKUPABLE_TO_PRODUCT_EFFICIENCY = global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL;

	private static float CALORIES_PER_KG_OF_CORAL_PICKUPABLE = ParrotFishTuning.STANDARD_CALORIES_PER_CYCLE / BaseParrotFish.KG_CORAL_PICKUPABLE_EATEN_PER_CYCLE;

	private static float CORAL_PLANT_PER_FISH = BaseParrotFish.KG_CORAL_PICKUPABLE_EATEN_PER_CYCLE / 20f;

	private static float CORAL_GROWTH_EATEN_PER_CYCLE = 0.25f * BaseParrotFish.CORAL_PLANT_PER_FISH;

	private static float CALORIES_PER_GROWTH_EATEN = ParrotFishTuning.STANDARD_CALORIES_PER_CYCLE / (BaseParrotFish.CORAL_GROWTH_EATEN_PER_CYCLE * 4f);

	private static float GROWTH_TO_PRODUCT_EFFICIENCY = BaseParrotFish.CORAL_PICKUPABLE_TO_PRODUCT_EFFICIENCY * 20f;

	private static float MIN_POOP_SIZE_IN_KG = BaseParrotFish.KG_CORAL_PICKUPABLE_EATEN_PER_CYCLE * BaseParrotFish.CORAL_PICKUPABLE_TO_PRODUCT_EFFICIENCY * 0.9f;
}
