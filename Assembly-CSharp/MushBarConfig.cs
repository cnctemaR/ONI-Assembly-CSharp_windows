using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MushBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("MushBar", ITEMS.FOOD.MUSHBAR.NAME, ITEMS.FOOD.MUSHBAR.DESC, 1f, false, Assets.GetAnim("mushbar_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.MUSHBAR, true);
		gameObject.AddComponent<DiseaseTrigger>().AddTrigger(GameHashes.EatCompleteEdible, new string[] { Db.Get().Diseases.Diarrhea.Id });
		string text = ITEMS.FOOD.MUSHBAR.RECIPEDESC;
		Recipe recipe = new Recipe("MushBar", 1f, (SimHashes)0, null, text, 1).SetFabricator("MicrobeMusher", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("Dirt", 75f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 75f));
		recipe.FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static GameObject CreateFabricationVisualizer(GameObject result)
	{
		KBatchedAnimController component = result.GetComponent<KBatchedAnimController>();
		GameObject gameObject = new GameObject();
		gameObject.name = "Visualizer";
		gameObject.SetActive(false);
		gameObject.transform.localPosition = Vector3.zero;
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.SetAnims(component.GetAnims(), true);
		kbatchedAnimController.initialAnim = "fabricating";
		kbatchedAnimController.isMovable = true;
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = new HashedString("meter_ration");
		kbatchedAnimTracker.offset = Vector3.zero;
		kbatchedAnimTracker.skipInitialDisable = true;
		return gameObject;
	}

	public const string ID = "MushBar";
}
