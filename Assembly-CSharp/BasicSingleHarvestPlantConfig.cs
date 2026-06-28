using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicSingleHarvestPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BasicSingleHarvestPlant", global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DESC, 400f, "meallice", "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, true, 15f, 5f, 273f, 283f, 303f, 315f, 0f, 0.15f, 1f, "BasicPlantFood");
		gameObject.UpdateComponentRequirement<BasicSingleHarvestPlant>(true);
		gameObject.UpdateComponentRequirement<KAnimControllerBase>(true).randomiseLoopedOffset = true;
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, "BasicSingleHarvestPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.DESC, "seed_meallice", new List<Tag> { GameTags.CropSeed }, default(Tag), 1, global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DOMESTICATEDDESC);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static float FERTILIZATION_RATE = 0.033333335f;
}
