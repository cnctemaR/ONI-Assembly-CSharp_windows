using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class HerbalRemedyConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("HerbalRemedy", ITEMS.PILLS.HERBALREMEDY.NAME, ITEMS.PILLS.HERBALREMEDY.DESC, 1f, true, Assets.GetAnim("pill_2_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.POLYGONAL, 1f, 1f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.HERBALREMEDY);
		string text = ITEMS.PILLS.HERBALREMEDY.NAME;
		string text2 = ITEMS.PILLS.HERBALREMEDY.RECIPEDESC;
		Recipe recipe = new Recipe("HerbalRemedy", 1f, (SimHashes)0, text, text2, 2).SetFabricator("Apothecary", 40f);
		recipe.AddIngredient(new Recipe.Ingredient("Sand", 100f));
		recipe.AddIngredient(new Recipe.Ingredient("Algae", 100f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HerbalRemedy";
}
