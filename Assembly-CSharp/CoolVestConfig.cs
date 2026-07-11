using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class CoolVestConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("BasicFabric", (float)global::TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS);
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.COOL_CLOTHING;
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Cool_Vest", global::TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, (float)global::TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS, global::TUNING.EQUIPMENT.VESTS.COOL_VEST_ICON0, global::TUNING.EQUIPMENT.VESTS.SNAPON0, global::TUNING.EQUIPMENT.VESTS.COOL_VEST_ANIM0, 4, list, global::TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f, null, null);
		Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.COOL_CLOTHING.conductivityMod)), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.COOL_CLOTHING.conductivityMod)), Descriptor.DescriptorType.Effect, false);
		Descriptor descriptor2 = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.COOL_CLOTHING.decorMod), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.COOL_CLOTHING.decorMod), Descriptor.DescriptorType.Effect, false);
		equipmentDef.additionalDescriptors.Add(descriptor);
		equipmentDef.additionalDescriptors.Add(descriptor2);
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			CoolVestConfig.OnEquipVest(eq, clothingInfo);
		};
		equipmentDef.OnUnequipCallBack = new Action<Equippable>(CoolVestConfig.OnUnequipVest);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC;
		return equipmentDef;
	}

	public static void OnEquipVest(Equippable eq, ClothingWearer.ClothingInfo clothingInfo)
	{
		if (eq == null || eq.assignee == null)
		{
			return;
		}
		Ownables soleOwner = eq.assignee.GetSoleOwner();
		if (soleOwner == null)
		{
			return;
		}
		MinionAssignablesProxy component = soleOwner.GetComponent<MinionAssignablesProxy>();
		ClothingWearer component2 = (component.target as KMonoBehaviour).GetComponent<ClothingWearer>();
		if (component2 != null)
		{
			component2.ChangeClothes(clothingInfo);
		}
		else
		{
			global::Debug.LogWarning("Clothing item cannot be equipped to assignee because they lack ClothingWearer component");
		}
	}

	public static void OnUnequipVest(Equippable eq)
	{
		if (eq != null && eq.assignee != null)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				ClothingWearer component = soleOwner.GetComponent<ClothingWearer>();
				if (component != null)
				{
					component.ChangeToDefaultClothes();
				}
			}
		}
	}

	public static void SetupVest(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
		Equippable equippable = go.GetComponent<Equippable>();
		if (equippable == null)
		{
			equippable = go.AddComponent<Equippable>();
		}
		equippable.SetQuality(global::QualityLevel.Poor);
		go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
	}

	public void DoPostConfigure(GameObject go)
	{
		CoolVestConfig.SetupVest(go);
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable, false);
	}

	public const string ID = "Cool_Vest";

	public static ComplexRecipe recipe;
}
