using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class HatchConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Hatch", global::STRINGS.CREATURES.SPECIES.HATCH.NAME, global::STRINGS.CREATURES.SPECIES.HATCH.DESC, 400f, "hatch", "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, false, FactionManager.FactionID.Pest, 25f, "HatchNavGrid", NavType.Floor, 2f, "Meat", 2, true, false, 30f, 283f, 294f, 273f, 315f);
		gameObject.UpdateComponentRequirement<Hatch>(true);
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		gameObject.UpdateComponentRequirement<SimpleMover>(true);
		gameObject.AddElementEmitter(SimHashes.Carbon, 0f, 0f);
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

	public const int unitsPerFeeding = 50;

	public const int rationsPerFeeding = 5;

	public const float maxHunger = 900f;

	public const float hungerEatThreshold = 600f;

	public const float minPoopSize = 100f;

	public const float maxPoopSize = 125f;

	public const SimHashes poopElement = SimHashes.Carbon;

	public const float burrowHardnessLimit = 20f;
}
