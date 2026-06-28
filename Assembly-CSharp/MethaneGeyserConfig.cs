using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MethaneGeyserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MethaneGeyser", global::STRINGS.CREATURES.SPECIES.METHANEGEYSER.NAME, global::STRINGS.CREATURES.SPECIES.METHANEGEYSER.DESC, 2000f, Assets.GetAnim("geyser_methane_kanim"), "inactive", Grid.SceneLayer.BuildingBack, 3, 3, tier, SimHashes.Creature, null);
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
		geyser.preEmissionElement = new Geyser.EmissionType(100f, new ElementConverter.OutputElement(0.06f, SimHashes.Methane, 423.15f, false, 1f, 1f, false));
		geyser.emissionElement = new Geyser.EmissionType(370f, new ElementConverter.OutputElement(0.12162162f, SimHashes.Methane, 368.15f, false, 1f, 1f, false));
		geyser.postEmissionElement = new Geyser.EmissionType(30f, new ElementConverter.OutputElement(0.3f, SimHashes.Methane, 368.15f, false, 1f, 1f, false));
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

	private const float EMIT_DURATION = 370f;

	private const float POST_DURATION = 30f;

	private const float PRE_METHANE_MASS = 6f;

	private const float EMIT_METHANE_MASS = 45f;

	private const float POST_METHANE_MASS = 9f;

	public const string ID = "MethaneGeyser";
}
