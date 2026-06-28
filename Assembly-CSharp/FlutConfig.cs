using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FlutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Flut", global::STRINGS.CREATURES.SPECIES.FLUT.NAME, global::STRINGS.CREATURES.SPECIES.FLUT.DESC, 25f, "flut_single", "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, false, FactionManager.FactionID.Prey, 25f, "SwimmerNavGrid", NavType.Swim, 2f, "Meat", 2, false, true, 30f, 283f, 294f, 273f, 315f);
		gameObject.UpdateComponentRequirement<Flut>(true);
		gameObject.UpdateComponentRequirement<Catchable>(true);
		gameObject.UpdateComponentRequirement<Storage>(true);
		gameObject.UpdateComponentRequirement<Operational>(true);
		ElementConverter elementConverter = gameObject.UpdateComponentRequirement<ElementConverter>(true);
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 0.25f, SimHashes.Fertilizer, 0f, false, 0f, 0f)
		};
		elementConverter.conversionInterval = 150f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddAnimController("flut_single", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
		gameObject.AddAquaticReproducer("flutEgg".ToTag(), 60f, 100f, 0.08f);
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

	public const float ReproductiveCylceLength = 60f;

	public const float RandomCycleOffset = 100f;

	public const float MaxmimumPopulationDensityForReproduce = 0.08f;
}
