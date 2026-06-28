using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FlutEggConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("FlutEgg", global::STRINGS.CREATURES.SPECIES.FLUTEGG.NAME, global::STRINGS.CREATURES.SPECIES.FLUTEGG.DESC, 25f, "flut_egg", "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, false, FactionManager.FactionID.Prey, 25f, "SwimmerNavGrid", NavType.Swim, 0f, "Flut", 1, false, true, 30f, 283f, 294f, 273f, 315f);
		gameObject.UpdateComponentRequirement<FlutEgg>(true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const float HatchTime = 100f;
}
