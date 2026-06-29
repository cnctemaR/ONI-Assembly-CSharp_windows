using System;
using TUNING;
using UnityEngine;

public static class BasePuftConfig
{
	public static GameObject BasePuft(string id, string name, string desc, string traitId, string anim_file, bool is_baby, string symbol_override_prefix)
	{
		float num = 50f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string text = "idle_loop";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, anim, text, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, traitId, "FlyerNavGrid1x1", NavType.Hover, 32, 2f, "Meat", 1, true, true, 30f, 302f, 318f, 243f, 343f);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbol_override_prefix, 0);
		}
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Creatures.Flyer);
		gameObject.AddOrGet<LoopingSounds>();
		LureableMonitor.Def def = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[] { GameTags.SlimeMold };
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_intake", NOISE_POLLUTION.CREATURES.TIER4);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_toot", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_inflated", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		string text2 = "Puft_air_intake";
		if (is_baby)
		{
			text2 = "PuftBaby_air_intake";
		}
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new TrappedStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new GrowUpStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new UpTopPoopStates.Def(), true)
			.Add(new InhaleStates.Def
			{
				inhaleSound = text2
			}, true)
			.Add(new LayEggStates.Def(), true)
			.Add(new MoveToLureStates.Def(), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup();
		IdleStates.Def def2 = new IdleStates.Def();
		def2.customIdleAnim = delegate(IdleStates.Instance isi)
		{
			CreatureCalorieMonitor.Instance smi = isi.GetSMI<CreatureCalorieMonitor.Instance>();
			return (smi == null || !smi.stomach.IsReadyToPoop()) ? "idle_loop" : "idle_loop_full";
		};
		ChoreTable.Builder builder2 = builder.Add(def2, true);
		EntityTemplates.AddCreatureBrain(gameObject, builder2, GameTags.Creatures.Species.PuftSpecies, symbol_override_prefix);
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

	public static void OnSpawn(GameObject inst)
	{
		Navigator component = inst.GetComponent<Navigator>();
		component.transitionDriver.overrideLayers.Add(new FullPuftTransitionLayer(component));
	}
}
