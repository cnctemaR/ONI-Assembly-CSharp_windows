using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EquipmentConfigManager")]
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
		if (!DlcManager.IsDlcListValidForCurrentContent(config.GetDlcIds()))
		{
			return;
		}
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		GameObject gameObject = EntityTemplates.CreateLooseEntity(equipmentDef.Id, equipmentDef.Name, equipmentDef.RecipeDescription, equipmentDef.Mass, true, equipmentDef.Anim, "object", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, true, 0, equipmentDef.OutputElement, null);
		Equippable equippable = gameObject.AddComponent<Equippable>();
		equippable.def = equipmentDef;
		global::Debug.Assert(equippable.def != null);
		equippable.slotID = equipmentDef.Slot;
		global::Debug.Assert(equippable.slot != null);
		config.DoPostConfigure(gameObject);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		if (equipmentDef.wornID != null)
		{
			GameObject gameObject2 = EntityTemplates.CreateLooseEntity(equipmentDef.wornID, equipmentDef.WornName, equipmentDef.WornDesc, equipmentDef.Mass, true, equipmentDef.Anim, "worn_out", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, true, 0, SimHashes.Creature, null);
			RepairableEquipment repairableEquipment = gameObject2.AddComponent<RepairableEquipment>();
			repairableEquipment.def = equipmentDef;
			global::Debug.Assert(repairableEquipment.def != null);
			SymbolOverrideControllerUtil.AddToPrefab(gameObject2);
			foreach (Tag tag in equipmentDef.AdditionalTags)
			{
				gameObject2.GetComponent<KPrefabID>().AddTag(tag, false);
			}
			Assets.AddPrefab(gameObject2.GetComponent<KPrefabID>());
		}
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		Recipe recipe = new Recipe(def.Id, 1f, (SimHashes)0, null, def.RecipeDescription, 0);
		recipe.SetFabricator(def.FabricatorId, def.FabricationTime);
		recipe.TechUnlock = def.RecipeTechUnlock;
		foreach (KeyValuePair<string, float> keyValuePair in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(keyValuePair.Key, keyValuePair.Value));
		}
	}

	public static EquipmentConfigManager Instance;
}
