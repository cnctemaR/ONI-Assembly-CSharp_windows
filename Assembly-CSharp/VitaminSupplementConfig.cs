using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class VitaminSupplementConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("VitaminSupplement", ITEMS.PILLS.VITAMINSUPPLEMENT.NAME, ITEMS.PILLS.VITAMINSUPPLEMENT.DESC, 1f, true, Assets.GetAnim("pill_2_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.VITAMINSUPPLEMENT);
		string text = ITEMS.PILLS.VITAMINSUPPLEMENT.NAME;
		string text2 = ITEMS.PILLS.VITAMINSUPPLEMENT.RECIPEDESC;
		Recipe recipe = new Recipe("VitaminSupplement", 1f, (SimHashes)0, text, text2, 10).SetFabricator("Apothecary", 40f);
		recipe.AddIngredient(new Recipe.Ingredient("Carbon", 100f));
		recipe.AddIngredient(new Recipe.Ingredient(SwampLilyFlowerConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "VitaminSupplement";
}
