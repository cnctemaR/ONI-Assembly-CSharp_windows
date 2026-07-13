using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseDivergentConfig
{
	public static GameObject BaseDivergent(string id, string name, string desc, float mass, string anim_file, string build_file, string traitId, bool is_baby, float num_tended_per_cycle = 8f, string symbolOverridePrefix = null, string cropTendingEffect = "DivergentCropTended", int meatAmount = 1, bool is_pacifist = true, string emoteAnim = "critter_emotes_kanim")
	{
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		string text = "WalkerNavGrid1x1";
		if (is_baby)
		{
			text = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(false, gameObject, anim_file, is_baby ? null : build_file, null, FactionManager.FactionID.Pest, traitId, text, NavType.Floor, 32, 2f, "Meat", (float)meatAmount, true, false, 283.15f, 313.15f, 243.15f, 373.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["DivergentBeetle"];
		pickupable.sortOrder = num;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<BurrowMonitor.Def>();
		gameObject.AddOrGetDef<CropTendingMonitor.Def>().numCropsTendedPerCycle = num_tended_per_cycle;
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		CropTendingStates.Def def = new CropTendingStates.Def();
		def.effectId = cropTendingEffect;
		def.interests.Add("WormPlant", 10);
		def.animSetOverrides.Add("WormPlant", new CropTendingStates.AnimSet
		{
			crop_tending_pre = "wormwood_tending_pre",
			crop_tending = "wormwood_tending",
			crop_tending_pst = "wormwood_tending_pst",
			hide_symbols_after_pre = new string[] { "flower", "flower_wilted" }
		});
		def.ignoreEffectGroup = PollinationMonitor.PollinationEffects;
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), !is_baby && !is_pacifist, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(def, !is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.Add(new CritterEmoteStates.Def(Assets.GetAnim(emoteAnim)), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.DivergentSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static List<Diet.Info> BasicSulfurDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Sulfur.CreateTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false, null)
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

	public const float CROP_TENDED_MULTIPLIER_DURATION = 600f;

	public const float CROP_TENDED_MULTIPLIER_EFFECT = 0.05f;

	public const string DEFAULT_CROP_TENDING_EFFECT = "DivergentCropTended";

	public static string[] ignoreEffectGroup = new string[] { "DivergentCropTended", "DivergentCropTendedWorm" };
}
