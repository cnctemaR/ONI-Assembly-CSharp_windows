using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FlutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Flut", global::STRINGS.CREATURES.SPECIES.FLUT.NAME, global::STRINGS.CREATURES.SPECIES.FLUT.DESC, 25f, Assets.GetAnim("flut_single_kanim"), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, 25f, "SwimmerNavGrid", NavType.Swim, 2f, "Meat", 2, false, true, 30f, 283f, 294f, 243f, 343f);
		gameObject.UpdateComponentRequirement<Flut>(true);
		gameObject.UpdateComponentRequirement<Catchable>(true);
		gameObject.UpdateComponentRequirement<Storage>(true);
		gameObject.UpdateComponentRequirement<Operational>(true);
		ElementConverter elementConverter = gameObject.UpdateComponentRequirement<ElementConverter>(true);
		elementConverter.conversionInterval = 150f;
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.25f / elementConverter.conversionInterval, SimHashes.Fertilizer, 0f, false, 0f, 0.5f, false)
		};
		KBatchedAnimController kbatchedAnimController = gameObject.AddAnimController("flut_single_kanim", Grid.SceneLayer.Front);
		kbatchedAnimController.isMovable = true;
		gameObject.AddAquaticReproducer("flutEgg".ToTag(), 60f, 100f, 0.08f);
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
