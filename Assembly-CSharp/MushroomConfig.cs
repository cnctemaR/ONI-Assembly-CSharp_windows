using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MushroomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(MushroomConfig.ID, ITEMS.FOOD.MUSHROOM.NAME, ITEMS.FOOD.MUSHROOM.DESC, 1f, false, Assets.GetAnim("funguscap_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.77f, 0.48f, true, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.MUSHROOM);
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
			int num = 0;
			float unitsConsumed = edible.unitsConsumed;
			int num2 = Mathf.FloorToInt(unitsConsumed);
			float num3 = unitsConsumed % 1f;
			if (global::UnityEngine.Random.value < num3)
			{
				num2++;
			}
			for (int i = 0; i < num2; i++)
			{
				if (global::UnityEngine.Random.value < MushroomConfig.SEEDS_PER_FRUIT_CHANCE)
				{
					num++;
				}
			}
			if (num > 0)
			{
				Vector3 vector = edible.transform.GetPosition() + new Vector3(0f, 0.05f, 0f);
				vector = Grid.CellToPosCCC(Grid.PosToCell(vector), Grid.SceneLayer.Ore);
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("MushroomSeed")), vector, Grid.SceneLayer.Ore, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
				PrimaryElement component = edible.GetComponent<PrimaryElement>();
				PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
				component2.Temperature = component.Temperature;
				component2.Units = (float)num;
				gameObject.SetActive(true);
			}
		}
	}

	public static float SEEDS_PER_FRUIT_CHANCE = 0.05f;

	public static string ID = "Mushroom";
}
