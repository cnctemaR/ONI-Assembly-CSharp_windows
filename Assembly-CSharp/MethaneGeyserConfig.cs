using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MethaneGeyserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MethaneGeyser", global::STRINGS.CREATURES.SPECIES.METHANEGEYSER.NAME, global::STRINGS.CREATURES.SPECIES.METHANEGEYSER.DESC, 2000f, Assets.GetAnim("geyser_side_methane_kanim"), "inactive", Grid.SceneLayer.BuildingBack, 4, 2, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER5, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		Geyser geyser = gameObject.AddComponent<Geyser>();
		geyser.idleDuration = 100f;
		ElementEmitter elementEmitter = gameObject.AddComponent<ElementEmitter>();
		elementEmitter.emitRange = 2;
		elementEmitter.maxPressure = 5f;
		geyser.SetEmitter(elementEmitter);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		geyser.emission_a = new Geyser.EmissionType(60f, 60f, 60f, new ElementConverter.OutputElement(0.75f, SimHashes.Methane, 423.15f, false, 1f, 1f, false, 1f, byte.MaxValue, 0), string.Empty);
		geyser.emission_b = geyser.emission_a;
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private const float IDLE_DURATION = 100f;

	private const float PRE_DURATION = 100f;

	private const float ERUPT_DURATION = 60f;

	private const float PST_DURATION = 10f;

	private const float EMIT_METHANE_MASS = 45f;

	public const string ID = "MethaneGeyser";
}
