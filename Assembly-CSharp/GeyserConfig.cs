using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeyserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Geyser", global::STRINGS.CREATURES.SPECIES.GEYSER.NAME, global::STRINGS.CREATURES.SPECIES.GEYSER.DESC, 2000f, Assets.GetAnim("geyser_kanim"), "inactive", Grid.SceneLayer.Background, 3, 3, tier, SimHashes.Creature, null);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		Geyser geyser = gameObject.AddComponent<Geyser>();
		geyser.idleDuration = 90f;
		ElementEmitter elementEmitter = gameObject.AddComponent<ElementEmitter>();
		geyser.SetEmitter(elementEmitter);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		geyser.preEmissionElement = new Geyser.EmissionType(30f, new ElementConverter.OutputElement(0.6666667f, SimHashes.Steam, 423.15f, false, 0f, 0f, false), 5f);
		geyser.emissionElement = new Geyser.EmissionType(10f, new ElementConverter.OutputElement(60f, SimHashes.Water, 368.15f, false, 0f, 0f, false), 1000f);
		geyser.postEmissionElement = new Geyser.EmissionType(20f, new ElementConverter.OutputElement(0.5f, SimHashes.Steam, 368.15f, false, 0f, 0f, false), 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private const float IDLE_DURATION = 90f;

	private const float PRE_DURATION = 30f;

	private const float EMIT_DURATION = 10f;

	private const float POST_DURATION = 20f;

	private const float PRE_STEAM_MASS = 20f;

	private const float EMIT_WATER_MASS = 600f;

	private const float POST_STEAM_MASS = 10f;
}
