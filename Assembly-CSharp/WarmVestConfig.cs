using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class WarmVestConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("BasicFabric", (float)global::TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS);
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.WARM_CLOTHING;
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Warm_Vest", global::TUNING.EQUIPMENT.CLOTHING.SLOT, global::TUNING.EQUIPMENT.VESTS.FABRICATOR, global::TUNING.EQUIPMENT.VESTS.WARM_VEST_FABTIME, SimHashes.Carbon, dictionary, (float)global::TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS, global::TUNING.EQUIPMENT.VESTS.WARM_VEST_ICON0, global::TUNING.EQUIPMENT.VESTS.SNAPON0, global::TUNING.EQUIPMENT.VESTS.WARM_VEST_ANIM0, 3, list, global::TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f, null);
		Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.WARM_CLOTHING.conductivityMod)), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.WARM_CLOTHING.conductivityMod)), Descriptor.DescriptorType.Effect, false);
		Descriptor descriptor2 = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.WARM_CLOTHING.decorMod), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.WARM_CLOTHING.decorMod), Descriptor.DescriptorType.Effect, false);
		equipmentDef.additionalDescriptors.Add(descriptor);
		equipmentDef.additionalDescriptors.Add(descriptor2);
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			ClothingWearer component = eq.assignee.GetSoleOwner().GetComponent<ClothingWearer>();
			if (component != null)
			{
				component.ChangeClothes(clothingInfo);
			}
			else
			{
				global::Debug.LogWarning("Clothing item cannot be equipped to assignee because they lack ClothingWearer component", null);
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq != null && eq.assignee != null)
			{
				ClothingWearer component2 = eq.assignee.GetSoleOwner().GetComponent<ClothingWearer>();
				component2.ChangeToDefaultClothes();
			}
		};
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC;
		return equipmentDef;
	}

	public static void SetupVest(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Clothes);
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
		WarmVestConfig.SetupVest(go);
	}

	public const string ID = "Warm_Vest";
}
