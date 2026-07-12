using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseOilFloaterConfig
{
	public static GameObject BaseOilFloater(string id, string name, string desc, string anim_file, string traitId, float warnLowTemp, float warnHighTemp, float lethalLowTemp, float lethalHighTemp, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 50f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, (warnLowTemp + warnHighTemp) / 2f);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Hoverer, false);
		gameObject.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, "FloaterNavGrid", NavType.Hover, 32, 2f, "Meat", 2, true, false, warnLowTemp, warnHighTemp, lethalLowTemp, lethalHighTemp);
		if (!string.IsNullOrEmpty(symbolOverridePrefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num2 = CREATURES.SORTING.CRITTER_ORDER["Oilfloater"];
		pickupable.sortOrder = num2;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		string text = "OilFloater_intake_air";
		if (is_baby)
		{
			text = "OilFloaterBaby_intake_air";
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new InhaleStates.Def
			{
				inhaleSound = text
			}, true, -1)
			.Add(new DrinkMilkStates.Def(), true, -1)
			.Add(new SameSpotPoopStates.Def(), true, -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def(), !is_baby, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.OilFloaterSpecies, symbolOverridePrefix);
		string text2 = "OilFloater_move_LP";
		if (is_baby)
		{
			text2 = "OilFloaterBaby_move_LP";
		}
		gameObject.AddOrGet<OilFloaterMovementSound>().sound = text2;
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, Tag consumed_tag, Tag producedTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced, float minPoopSizeInKg)
	{
		Diet diet = new Diet(new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag> { consumed_tag }, producedTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, false, false)
		});
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = minPoopSizeInKg * caloriesPerKg;
		prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}
}
