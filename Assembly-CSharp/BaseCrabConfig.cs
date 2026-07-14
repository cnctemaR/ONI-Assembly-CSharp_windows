using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseCrabConfig
{
	public static GameObject BaseCrab(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null, string onDeathDropID = "CrabShell", float onDeathDropCount = 1f)
	{
		string[] array;
		if (!string.IsNullOrEmpty(onDeathDropID))
		{
			(array = new string[1])[0] = onDeathDropID;
		}
		else
		{
			array = null;
		}
		return BaseCrabConfig.BaseCrab(id, name, desc, anim_file, traitId, is_baby, symbolOverridePrefix, array, new float[] { onDeathDropCount });
	}

	public static GameObject BaseCrab(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix, string[] onDeathDropsID, float[] onDeathDropsCount)
	{
		float num = 100f;
		int num2 = (is_baby ? 1 : 2);
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(is_baby ? anim_file : "pincher_build_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, num2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		string text = "WalkerNavGrid1x2";
		if (is_baby)
		{
			text = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(new EntityTemplates.ExtendEntityToBasicCreatureData
		{
			isWarmBlooded = false,
			template = gameObject,
			anim_filename = anim_file,
			build_filename = (is_baby ? null : "pincher_build_kanim"),
			symbol_override_prefix = symbolOverridePrefix,
			faction = FactionManager.FactionID.Pest,
			initialTraitID = traitId,
			NavGridName = text,
			onDeathDropsID = onDeathDropsID,
			onDeathDropsCount = onDeathDropsCount,
			entombVulnerable = false,
			drownVulnerable = false,
			warningLowTemperature = 273.15f,
			warningHighTemperature = 313.15f,
			lethalLowTemperature = 223.15f,
			lethalHighTemperature = 373.15f
		});
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		int num3 = global::TUNING.CREATURES.SORTING.CRITTER_ORDER["Crab"];
		pickupable.sortOrder = num3;
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		ThreatMonitor.Def def = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def.fleethresholdState = Health.HealthState.Dead;
		def.friendlyCreatureTags = new Tag[] { GameTags.Creatures.CrabFriend };
		def.maxSearchDistance = 12;
		def.offsets = CrabTuning.DEFEND_OFFSETS;
		gameObject.AddWeapon(2f, 3f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker, false);
		component.AddTag(GameTags.Creatures.CrabFriend, false);
		KAnimFile anim = Assets.GetAnim("pincher_emotes_kanim");
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), is_baby, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), is_baby, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new DefendStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), true, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), !is_baby, -1)
			.Add(new LayEggStates.Def(), !is_baby, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new DrinkMilkStates.Def
			{
				shouldBeBehindMilkTank = true
			}, true, -1)
			.Add(new PoopStates.Def(anim, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, false), true, -1)
			.Add(new PunchClamOpenStates.Def(), !is_baby && DlcManager.IsContentSubscribed("DLC5_ID"), -1)
			.Add(new CallAdultStates.Def(), is_baby, -1)
			.Add(new CritterCondoStates.Def
			{
				entersBuilding = false
			}, !is_baby, -1)
			.Add(new CritterEmoteStates.Def(anim), true, -1)
			.PopInterruptGroup()
			.Add(new CreatureDiseaseCleaner.Def(30f), true, -1)
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.CrabSpecies, symbolOverridePrefix);
		CritterCondoInteractMontior.Def def2 = gameObject.AddOrGetDef<CritterCondoInteractMontior.Def>();
		def2.requireCavity = false;
		def2.condoPrefabTag = "UnderwaterCritterCondo";
		if (!is_baby && DlcManager.IsContentSubscribed("DLC5_ID"))
		{
			gameObject.AddOrGetDef<PunchClamMonitor.Def>();
		}
		gameObject.AddTag(GameTags.Amphibious);
		return gameObject;
	}

	public static List<Diet.Info> BasicDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.ToxicSand.CreateTag());
		hashSet.Add(RotPileConfig.ID.ToTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, Diet.Info.FoodType.EatSolid, false, null)
		};
	}

	public static List<Diet.Info> DietWithSlime(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.ToxicSand.CreateTag());
		hashSet.Add(RotPileConfig.ID.ToTag());
		hashSet.Add(SimHashes.SlimeMold.CreateTag());
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

	private static int AdjustSpawnLocationCB(int cell)
	{
		while (!Grid.Solid[cell])
		{
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(cell))
			{
				break;
			}
			cell = num;
		}
		return cell;
	}

	public const string EMOTION_FILE_NAME = "pincher_emotes_kanim";
}
