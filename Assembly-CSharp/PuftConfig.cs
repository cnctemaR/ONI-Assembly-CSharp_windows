using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Puft", global::STRINGS.CREATURES.SPECIES.PUFT.NAME, global::STRINGS.CREATURES.SPECIES.PUFT.DESC, 50f, "puft", "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, false, FactionManager.FactionID.Prey, 25f, "FlyerNavGrid", NavType.Hover, 2f, "Meat", 1, true, true, 30f, 302f, 318f, 295f, 325f);
		gameObject.UpdateComponentRequirement<Puft>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		gameObject.AddElementEmitter(SimHashes.SlimeMold, 0f, 0f);
		gameObject.AddElementConsumer(SimHashes.ContaminatedOxygen, 0.25f, 0.05f, 3);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabSpawnFn += delegate(GameObject go)
		{
			Navigator component2 = go.GetComponent<Navigator>();
			component2.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component2));
		};
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
