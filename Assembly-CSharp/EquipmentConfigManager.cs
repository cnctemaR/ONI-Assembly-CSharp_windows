using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentConfigManager : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		EquipmentConfigManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EquipmentConfigManager.Instance = this;
	}

	public void RegisterEquipment(IEquipmentConfig config)
	{
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		string id = equipmentDef.Id;
		string name = equipmentDef.Name;
		string recipeDescription = equipmentDef.RecipeDescription;
		float mass = equipmentDef.Mass;
		bool flag = true;
		KAnimFile anim = equipmentDef.Anim;
		string text = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		EntityTemplates.CollisionShape collisionShape = equipmentDef.CollisionShape;
		float width = equipmentDef.width;
		float height = equipmentDef.height;
		bool flag2 = true;
		SimHashes outputElement = equipmentDef.OutputElement;
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, recipeDescription, mass, flag, anim, text, sceneLayer, collisionShape, width, height, flag2, 0, outputElement, null);
		Equippable equippable = gameObject.AddComponent<Equippable>();
		equippable.def = equipmentDef;
		equippable.slotID = equipmentDef.Slot;
		config.DoPostConfigure(gameObject);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		string id = def.Id;
		string recipeDescription = def.RecipeDescription;
		Recipe recipe = new Recipe(id, 1f, (SimHashes)0, null, recipeDescription, 0);
		recipe.SetFabricator(def.FabricatorId, def.FabricationTime);
		recipe.TechUnlock = def.RecipeTechUnlock;
		foreach (KeyValuePair<string, float> keyValuePair in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(keyValuePair.Key, keyValuePair.Value));
		}
	}

	public static EquipmentConfigManager Instance;
}
