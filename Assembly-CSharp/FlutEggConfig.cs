using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FlutEggConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("FlutEgg", global::STRINGS.CREATURES.SPECIES.FLUTEGG.NAME, global::STRINGS.CREATURES.SPECIES.FLUTEGG.DESC, 25f, Assets.GetAnim("flut_egg_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, 25f, "SwimmerNavGrid", NavType.Swim, 0f, "Flut", 1, false, true, 30f, 283f, 294f, 243f, 343f);
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
