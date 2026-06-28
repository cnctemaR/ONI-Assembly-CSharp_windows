using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EquipmentConfigManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		EquipmentConfigManager.Instance = this;
		this.baseTemplate = new GameObject("BuildingTemplate");
		this.baseTemplate.SetActive(false);
		this.baseTemplate.AddComponent<KPrefabID>();
		this.baseTemplate.AddComponent<KSelectable>();
		this.baseTemplate.AddComponent<PrimaryElement>();
		this.baseTemplate.AddComponent<Equippable>();
		this.baseTemplate.AddComponent<Pickupable>();
		this.baseTemplate.AddComponent<SaveLoadRoot>();
		this.baseTemplate.AddComponent<Equipment>();
		MeshRenderer meshRenderer = this.baseTemplate.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Assets.AnimMaterial;
		meshRenderer.useLightProbes = false;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		KBatchedAnimController kbatchedAnimController = this.baseTemplate.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.initialAnim = "idle";
		CircleCollider2D circleCollider2D = this.baseTemplate.AddComponent<CircleCollider2D>();
		circleCollider2D.radius = 0.325f;
	}

	public void RegisterEquipment(IEquipmentConfig config)
	{
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.baseTemplate);
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.EquipmentTemplates).transform;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.PrefabTag = new Tag(equipmentDef.Id);
		KSelectable component2 = gameObject.GetComponent<KSelectable>();
		component2.name = equipmentDef.Name;
		component2.SetName(equipmentDef.Name);
		PrimaryElement component3 = gameObject.GetComponent<PrimaryElement>();
		component3.Mass = equipmentDef.Mass;
		component3.Temperature = 293f;
		component3.SetElement(ElementLoader.GetElementID(new Tag(equipmentDef.OutputElement)));
		Equippable component4 = gameObject.GetComponent<Equippable>();
		EquipmentSlot equipmentSlot = EquipmentSet.Get().slotSet.Get(equipmentDef.Slot);
		if (equipmentSlot == null)
		{
			Debug.LogError("Invalid slot: " + equipmentDef.Slot);
		}
		component4.def = equipmentDef;
		component4.slot = equipmentSlot;
		KBatchedAnimController component5 = gameObject.GetComponent<KBatchedAnimController>();
		component5.SetAnims(new KAnimFile[] { equipmentDef.Anim }, true);
		this.LoadRecipe(equipmentDef, component4);
		config.DoPostConfigure(gameObject);
		Assets.AddPrefab(component);
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		string recipeDescription = def.RecipeDescription;
		Recipe recipe = new Recipe(equippable.gameObject, def.FabricatorId, (float)def.FabricationTime, 1f, (SimHashes)0, null, recipeDescription, 0);
		foreach (KeyValuePair<string, float> keyValuePair in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(keyValuePair.Key, keyValuePair.Value));
		}
		RecipeManager.Get().Add(recipe);
	}

	public static EquipmentConfigManager Instance;

	private GameObject baseTemplate;
}
