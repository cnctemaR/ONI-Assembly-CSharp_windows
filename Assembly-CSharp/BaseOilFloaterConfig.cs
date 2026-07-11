using System;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseOilFloaterConfig
{
	public static GameObject BaseOilFloater(string id, string name, string desc, string anim_file, string traitId, float warnLowTemp, float warnHighTemp, bool is_baby, string symbolOverridePrefix = null)
	{
		float num = 400f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER1;
		float num2 = (warnLowTemp + warnHighTemp) / 2f;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, num2);
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.GroundBased);
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Pest;
		string text2 = "FloaterNavGrid";
		NavType navType = NavType.Hover;
		string text3 = "Meat";
		int num3 = 2;
		bool flag = false;
		bool flag2 = false;
		num2 = warnLowTemp - 15f;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, traitId, text2, navType, 32, 2f, text3, num3, flag, flag2, 30f, warnLowTemp, warnHighTemp, num2, warnHighTemp + 20f);
		if (!string.IsNullOrEmpty(symbolOverridePrefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbolOverridePrefix, 0);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		string text4 = id + "_Preview";
		EntityTemplates.CreateAndRegisterPreview(text4, Assets.GetAnim("oilfloater_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, name), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, name), Assets.GetAnim("creature_interacts_trap_oilfloater_kanim"), "working_pre", new Tag(text4), true);
		string text5 = "OilFloater_intake_air";
		if (is_baby)
		{
			text5 = "OilFloaterBaby_intake_air";
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new TrappedStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new SubmergedStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.ExitSubmerged, false, "idle_loop", global::STRINGS.CREATURES.STATUSITEMS.IDLE.NAME, global::STRINGS.CREATURES.STATUSITEMS.IDLE.TOOLTIP), true)
			.Add(new DebugGoToStates.Def(), true)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new GrowUpStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new InhaleStates.Def
			{
				inhaleSound = text5
			}, true)
			.Add(new LayEggStates.Def(), true)
			.Add(new SameSpotPoopStates.Def(), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.OilFloaterSpecies, symbolOverridePrefix);
		string move_sound = "OilFloater_move_LP";
		if (is_baby)
		{
			move_sound = "OilFloaterBaby_move_LP";
		}
		gameObject.AddOrGet<KPrefabID>().prefabSpawnFn += delegate(GameObject inst)
		{
			inst.Subscribe(1027377649, delegate(object data)
			{
				BaseOilFloaterConfig.OnObjectMovementStateChanged(inst, data, move_sound);
			});
		};
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, Tag consumedTag, Tag producedTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced, float minPoopSizeInKg)
	{
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(consumedTag, producedTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = minPoopSizeInKg * caloriesPerKg;
		GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}

	public static void OnObjectMovementStateChanged(GameObject inst, object data, string sound)
	{
	}
}
