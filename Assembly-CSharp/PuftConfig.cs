using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Puft", global::STRINGS.CREATURES.SPECIES.PUFT.NAME, global::STRINGS.CREATURES.SPECIES.PUFT.DESC, 50f, Assets.GetAnim("puft_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, 25f, "FlyerNavGrid", NavType.Hover, 2f, "Meat", 1, true, true, 30f, 302f, 318f, 243f, 343f);
		gameObject.UpdateComponentRequirement<Puft>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		ElementEmitter elementEmitter = gameObject.AddElementEmitter(SimHashes.SlimeMold, 0f, 0f);
		ElementConsumer elementConsumer = gameObject.AddElementConsumer(SimHashes.ContaminatedOxygen, 0.25f, 0.05f, 3);
		elementEmitter.showDescriptor = false;
		elementConsumer.showDescriptor = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const SimHashes consumeElement = SimHashes.ContaminatedOxygen;

	public const SimHashes emitElement = SimHashes.SlimeMold;

	public const float minimumApproachMass = 0.1f;
}
