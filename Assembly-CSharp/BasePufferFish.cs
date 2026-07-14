using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasePufferFish
{
	public static GameObject CreatePrefab(string id, string base_trait_id, string name, string description, string anim_file, bool is_baby, string symbol_prefix, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp)
	{
		float mass = PufferFishTuning.MASS;
		EffectorValues decor = PufferFishTuning.DECOR;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Creatures;
		int num = 1;
		int num2 = 1;
		EffectorValues effectorValues = decor;
		float num3 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, description, mass, anim, text, sceneLayer, num, num2, effectorValues, default(EffectorValues), SimHashes.Creature, null, num3);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.SwimmingCreature, false);
		component.AddTag(GameTags.Creatures.Swimmer, false);
		Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 2000000f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -666.6667f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, PufferFishTuning.HITPOINTS, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, PufferFishTuning.LIFESPAN, name, false, false, true));
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, false, true, true);
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : "blowfish_build_kanim", symbol_prefix, FactionManager.FactionID.Prey, base_trait_id, "SwimmerNavGrid", NavType.Swim, 32, 2f, "FishMeat", 1f, false, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		KAnimFile anim2 = Assets.GetAnim("blowfish_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def
			{
				getLandAnim = new Func<FallStates.Instance, string>(BasePufferFish.GetLandAnim)
			}, true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FlopStates.Def
			{
				frameToFlopStart = (is_baby ? 19 : 10),
				frameToFlopEnd = (is_baby ? 33 : 32)
			}, true, -1)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new VentBubbleStates.Def
			{
				element = SimHashes.Oxygen,
				dupebreathingAnimFiles = new KAnimFile[] { Assets.GetAnim("anim_interact_blowfish_kanim") },
				dupebreathingAnims = new HashedString[] { "blowfish_breath_pre", "blowfish_breathe_loop" },
				dupebreathingPst = new HashedString[] { "blowfish_breath_pst" }
			}, !is_baby, -1)
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
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.PufferFishSpecies, symbol_prefix);
		CritterCondoInteractMontior.Def def = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def.requireCavity = false;
		def.condoPrefabTag = "UnderwaterCritterCondo";
		new HashSet<Tag>();
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("Lettuce");
		HashSet<Tag> hashSet2 = new HashSet<Tag>();
		hashSet2.Add(SeaLettuceConfig.ID);
		string[] array = new string[] { "eat_pre", "eat_loop", "idle_loop" };
		Diet diet = new Diet(new List<Diet.Info>
		{
			new Diet.Info(hashSet, PufferFishTuning.POOP_ELEMENT, 400000f, 15f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, array),
			new Diet.Info(hashSet2, PufferFishTuning.POOP_ELEMENT, 400000f, 15f, null, 0f, false, Diet.Info.FoodType.EatPlantDirectly, false, array)
		}.ToArray());
		CreatureCalorieMonitor.Def def2 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def2.diet = diet;
		def2.minConsumedCaloriesBeforePooping = 200000f;
		def2.minimumTimeBeforePooping = 0f;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		Storage storage = gameObject.AddComponent<Storage>();
		storage.capacityKg = PufferFishTuning.OXYGEN_STORAGE_CAPACITY;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		gameObject.AddOrGet<UnderwaterBreathingLocation>().allowLandUse = false;
		gameObject.AddOrGet<UnderwaterBreathingLocationWorkable>();
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[] { GameTags.Creatures.FishTrapLure };
		if (!string.IsNullOrEmpty(symbol_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_prefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num4 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["PufferFish"];
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

	private const string EMOTION_FILE_NAME = "blowfish_emotes_kanim";
}
