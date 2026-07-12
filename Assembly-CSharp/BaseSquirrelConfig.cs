using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseSquirrelConfig
{
	public static GameObject BaseSquirrel(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null, bool isHuggable = false)
	{
		float num = 100f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, num, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		string text = "SquirrelNavGrid";
		if (is_baby)
		{
			text = "DreckoBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, text, NavType.Floor, 32, 2f, "Meat", 1, true, false, 283.15f, 293.15f, 243.15f, 343.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
		}
		gameObject.AddComponent<Storage>();
		if (!is_baby)
		{
			gameObject.AddOrGetDef<SeedPlantingMonitor.Def>();
			gameObject.AddOrGetDef<ClimbableTreeMonitor.Def>();
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
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
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		ChoreTable.Builder builder = new ChoreTable.Builder().Add(new DeathStates.Def(), true, -1).Add(new AnimInterruptStates.Def(), true, -1).Add(new GrowUpStates.Def(), true, -1)
			.Add(new TrappedStates.Def(), true, -1)
			.Add(new IncubatingStates.Def(), true, -1)
			.Add(new BaggedStates.Def(), true, -1)
			.Add(new FallStates.Def(), true, -1)
			.Add(new StunnedStates.Def(), true, -1)
			.Add(new DrowningStates.Def(), true, -1)
			.Add(new DebugGoToStates.Def(), true, -1)
			.Add(new FleeStates.Def(), true, -1)
			.Add(new AttackStates.Def("eat_pre", "eat_pst", null), true, -1)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true, -1)
			.Add(new FixedCaptureStates.Def(), true, -1)
			.Add(new RanchedStates.Def(), true, -1)
			.Add(new LayEggStates.Def(), true, -1)
			.Add(new HugEggStates.Def(GameTags.Creatures.WantsToTendEgg), isHuggable, -1)
			.Add(new HugMinionStates.Def(), isHuggable, -1)
			.Add(new TreeClimbStates.Def(), true, -1)
			.Add(new EatStates.Def(), true, -1)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true, -1)
			.Add(new CallAdultStates.Def(), true, -1)
			.Add(new SeedPlantingStates.Def(symbolOverridePrefix), true, -1)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true, -1);
		EntityTemplates.AddCreatureBrain(gameObject, builder, GameTags.Creatures.Species.SquirrelSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static Diet.Info[] BasicDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		return new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag>
			{
				"ForestTree",
				BasicFabricMaterialPlantConfig.ID
			}, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false, true)
		};
	}

	public static GameObject SetupDiet(GameObject prefab, Diet.Info[] diet_infos, float minPoopSizeInKg)
	{
		Diet diet = new Diet(diet_infos);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = minPoopSizeInKg;
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
}
