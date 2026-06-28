using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFruitConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(PrickleFruitConfig.ID, ITEMS.FOOD.PRICKLEFRUIT.NAME, ITEMS.FOOD.PRICKLEFRUIT.DESC, 1f, "bristleberry", "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.PRICKLEFRUIT);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.Subscribe(-10536414, delegate(object data)
		{
			this.OnEatComplete(inst.GetComponent<Edible>());
		});
	}

	private void OnEatComplete(Edible edible)
	{
		if (edible != null)
		{
			Vector3 vector = edible.transform.position + new Vector3(0f, 0.05f, 0f);
			vector = Grid.CellToPosCCC(Grid.PosToCell(vector), Grid.SceneLayer.Use);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(PrickleFlowerConfig.SEED_ID)), vector, Grid.SceneLayer.Use, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
			PrimaryElement component = edible.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			float num = edible.rationsConsumed / (float)edible.FoodInfo.Rations * PrickleFruitConfig.SEEDS_PER_FRUIT;
			int num2 = Mathf.FloorToInt(num);
			float num3 = num % 1f;
			if (global::UnityEngine.Random.value < num3)
			{
				num2++;
			}
			component2.Units = (float)num2;
			gameObject.SetActive(true);
		}
	}

	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "PrickleFruit";
}
