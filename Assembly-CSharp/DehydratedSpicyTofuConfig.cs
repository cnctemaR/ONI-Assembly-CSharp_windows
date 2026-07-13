using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class DehydratedSpicyTofuConfig : IEntityConfig
{
	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public GameObject CreatePrefab()
	{
		KAnimFile anim = Assets.GetAnim("dehydrated_food_spicy_tofu_kanim");
		GameObject gameObject = EntityTemplates.CreateLooseEntity(DehydratedSpicyTofuConfig.ID.Name, global::STRINGS.ITEMS.FOOD.SPICYTOFU.DEHYDRATED.NAME, global::STRINGS.ITEMS.FOOD.SPICYTOFU.DEHYDRATED.DESC, 1f, true, anim, "idle", Grid.SceneLayer.BuildingFront, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, 0, SimHashes.Polypropylene, null);
		EntityTemplates.ExtendEntityToDehydratedFoodPackage(gameObject, FOOD.FOOD_TYPES.SPICY_TOFU);
		return gameObject;
	}

	public static Tag ID = new Tag("DehydratedSpicyTofu");

	public const float MASS = 1f;

	public const string ANIM_FILE = "dehydrated_food_spicy_tofu_kanim";

	public const string INITIAL_ANIM = "idle";
}
