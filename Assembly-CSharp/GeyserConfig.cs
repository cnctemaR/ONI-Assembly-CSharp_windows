using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeyserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Geyser", global::STRINGS.CREATURES.SPECIES.GEYSER.NAME, global::STRINGS.CREATURES.SPECIES.GEYSER.DESC, 2000f, Assets.GetAnim("geyser_side_steam_kanim"), "inactive", Grid.SceneLayer.BuildingBack, 4, 2, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER6, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		Geyser geyser = gameObject.AddComponent<Geyser>();
		geyser.idleDuration = 40f;
		ElementEmitter elementEmitter = gameObject.AddComponent<ElementEmitter>();
		elementEmitter.emitRange = 2;
		elementEmitter.maxPressure = 5f;
		geyser.SetEmitter(elementEmitter);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		geyser.emission_a = new Geyser.EmissionType(20f, 20f, 10f, new ElementConverter.OutputElement(1.5f, SimHashes.Steam, 423.15f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0), string.Empty);
		geyser.emission_b = new Geyser.EmissionType(20f, 20f, 10f, new ElementConverter.OutputElement(36.25f, SimHashes.Water, 368.15f, false, 1f, 1f, false, 1f, byte.MaxValue, 0), "erupt_water");
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private const float IDLE_DURATION = 40f;

	private const float PRE_DURATION = 20f;

	private const float ERUPT_DURATION = 20f;

	private const float PST_DURATION = 10f;

	private const float EMIT_WATER_MASS = 725f;

	private const float EMIT_STEAM_MASS = 30f;
}
