using System;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

public class HatchConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Hatch", global::STRINGS.CREATURES.SPECIES.HATCH.NAME, global::STRINGS.CREATURES.SPECIES.HATCH.DESC, 400f, Assets.GetAnim("hatch_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, 25f, "HatchNavGrid", NavType.Floor, 2f, "Meat", 2, true, false, 30f, 283f, 294f, 243f, 343f);
		gameObject.UpdateComponentRequirement<Hatch>(true);
		Trappable trappable = gameObject.UpdateComponentRequirement<Trappable>(true);
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.Carbon, 0f, 0f, SimUtil.DiseaseInfo.Invalid);
		elementEmitter.showDescriptor = false;
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreview("Hatch_Preview", Assets.GetAnim("hatch_kanim"), "idle_loop", ObjectLayer.NumLayers, 1, 1);
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, string.Format(global::STRINGS.CREATURES.BAGGED_NAME_FMT, global::STRINGS.CREATURES.SPECIES.HATCH.NAME), string.Format(global::STRINGS.CREATURES.BAGGED_DESC_FMT, global::STRINGS.CREATURES.SPECIES.HATCH.NAME), Assets.GetAnim("creature_interacts_trap_hatch_kanim"), "working_pre", new Tag("Hatch_Preview"));
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Hatch";

	public const string PREVIEW_ID = "Hatch_Preview";

	public const float unitsPerFeeding = 50f;

	public const float foodUnitsPerFeeding = 0.5f;

	public const float maxHunger = 900f;

	public const float hungerEatThreshold = 600f;

	public const float minPoopSize = 100f;

	public const float maxPoopSize = 125f;

	public const SimHashes poopElement = SimHashes.Carbon;

	public const float burrowHardnessLimit = 20f;
}
