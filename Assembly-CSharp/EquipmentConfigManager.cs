using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentConfigManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EquipmentConfigManager.Instance = this;
	}

	public void RegisterEquipment(IEquipmentConfig config)
	{
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		GameObject gameObject = EntityTemplates.CreateLooseEntity(equipmentDef.Id, equipmentDef.Name, equipmentDef.RecipeDescription, equipmentDef.Mass, true, equipmentDef.Anim, "object", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, true, equipmentDef.OutputElement, null);
		Equippable equippable = gameObject.AddComponent<Equippable>();
		equippable.def = equipmentDef;
		equippable.slotID = equipmentDef.Slot;
		this.LoadRecipe(equipmentDef, equippable);
		config.DoPostConfigure(gameObject);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		string id = def.Id;
		string recipeDescription = def.RecipeDescription;
		Recipe recipe = new Recipe(id, 1f, (SimHashes)0, null, recipeDescription, 0).SetFabricator(def.FabricatorId, def.FabricationTime);
		foreach (KeyValuePair<string, float> keyValuePair in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(keyValuePair.Key, keyValuePair.Value));
		}
	}

	public static EquipmentConfigManager Instance;
}
