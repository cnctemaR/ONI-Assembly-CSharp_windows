using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class DehydratedSurfAndTurfConfig : IEntityConfig
{
	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public GameObject CreatePrefab()
	{
		KAnimFile anim = Assets.GetAnim("dehydrated_food_surf_and_turf_kanim");
		GameObject gameObject = EntityTemplates.CreateLooseEntity(DehydratedSurfAndTurfConfig.ID.Name, global::STRINGS.ITEMS.FOOD.SURFANDTURF.DEHYDRATED.NAME, global::STRINGS.ITEMS.FOOD.SURFANDTURF.DEHYDRATED.DESC, 1f, true, anim, "idle", Grid.SceneLayer.BuildingFront, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, 0, SimHashes.Polypropylene, null);
		EntityTemplates.ExtendEntityToDehydratedFoodPackage(gameObject, FOOD.FOOD_TYPES.SURF_AND_TURF);
		return gameObject;
	}

	public static Tag ID = new Tag("DehydratedSurfAndTurf");

	public const float MASS = 1f;

	public const int FABRICATION_TIME_SECONDS = 300;

	public const string ANIM_FILE = "dehydrated_food_surf_and_turf_kanim";

	public const string INITIAL_ANIM = "idle";
}
