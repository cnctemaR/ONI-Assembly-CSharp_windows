using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MushBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("MushBar", ITEMS.FOOD.MUSHBAR.NAME, ITEMS.FOOD.MUSHBAR.DESC, 1f, "mushbar_kanim", "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.MUSHBAR);
		gameObject.AddComponent<DiseaseTrigger>().AddTrigger(GameHashes.EatCompleteEdible, new string[] { Db.Get().Diseases.Diarrhea.Id });
		string text = ITEMS.FOOD.MUSHBAR.RECIPEDESC;
		Recipe recipe = new Recipe(gameObject, "MicrobeMusher", 40f, 1f, (SimHashes)0, null, text, 1);
		recipe.AddIngredient(new Recipe.Ingredient("Dirt", 75f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 75f));
		RecipeManager.Get().Add(recipe);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static void DoPostConfigure(GameObject go)
	{
		IEffectDescriptor[] components = go.GetComponents<IEffectDescriptor>();
		foreach (IEffectDescriptor effectDescriptor in components)
		{
			int num = 9999;
			effectDescriptor.DescriptionOrder = num;
			KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)effectDescriptor;
			string name = kmonoBehaviour.GetType().Name;
			for (int j = 0; j < global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.Length; j++)
			{
				if (global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER[j] == name)
				{
					effectDescriptor.DescriptionOrder = j;
					break;
				}
			}
			if (effectDescriptor.DescriptionOrder == num)
			{
				Debug.LogWarning("Missing Effect Descriptor Order: " + name);
			}
		}
	}
}
