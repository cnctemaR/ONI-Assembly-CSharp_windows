using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuperWormPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = WormPlantConfig.BaseWormPlant("SuperWormPlant", global::STRINGS.CREATURES.SPECIES.SUPERWORMPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SUPERWORMPLANT.DESC, "wormwood_kanim", SuperWormPlantConfig.SUPER_DECOR, "WormSuperFruit");
		gameObject.AddOrGet<SeedProducer>().Configure("WormPlantSeed", SeedProducer.ProductionType.Harvest, 1);
		gameObject.AddOrGet<PlantFiberProducer>().amount = 16f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		TransformingPlant transformingPlant = prefab.AddOrGet<TransformingPlant>();
		transformingPlant.SubscribeToTransformEvent(GameHashes.HarvestComplete);
		transformingPlant.transformPlantId = "WormPlant";
		prefab.GetComponent<KAnimControllerBase>().SetSymbolVisiblity("flower", false);
		prefab.AddOrGet<StandardCropPlant>().anims = SuperWormPlantConfig.animSet;
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SuperWormPlant";

	public static readonly EffectorValues SUPER_DECOR = DECOR.BONUS.TIER1;

	public const string SUPER_CROP_ID = "WormSuperFruit";

	public const int CROP_YIELD = 8;

	public const float PLANT_FIBER_PRODUCED_PER_CYCLE = 16f;

	private static StandardCropPlant.AnimSet animSet = new StandardCropPlant.AnimSet
	{
		grow = "super_grow",
		grow_pst = "super_grow_pst",
		idle_full = "super_idle_full",
		wilt_base = "super_wilt",
		harvest = "super_harvest"
	};
}
