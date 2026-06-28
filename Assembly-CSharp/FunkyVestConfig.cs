using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class FunkyVestConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add("BasicFabric", (float)global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS);
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.FANCY_CLOTHING;
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Funky_Vest", global::TUNING.EQUIPMENT.SUITS.SLOT, global::TUNING.EQUIPMENT.VESTS.FABRICATOR, global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME, SimHashes.Carbon, dictionary, (float)global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS, global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_ICON0, global::TUNING.EQUIPMENT.VESTS.SNAPON0, global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_ANIM0, PathFinderFlags.None, list, global::TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f);
		Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), Descriptor.DescriptorType.Effect, false);
		Descriptor descriptor2 = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), Descriptor.DescriptorType.Effect, false);
		equipmentDef.additionalDescriptors.Add(descriptor);
		equipmentDef.additionalDescriptors.Add(descriptor2);
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			ClothingWearer component = eq.assignee.GetComponent<ClothingWearer>();
			if (component != null)
			{
				component.ChangeClothes(clothingInfo);
			}
			else
			{
				Debug.LogWarning("Clothing item cannot be equipped to assignee because they lack ClothingWearer component");
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq != null && eq.assignee != null)
			{
				ClothingWearer component2 = eq.assignee.GetComponent<ClothingWearer>();
				component2.ChangeToDefaultClothes();
			}
		};
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC;
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
		Equipment equipment = go.GetComponent<Equipment>();
		if (equipment == null)
		{
			equipment = go.AddComponent<Equipment>();
		}
		go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
	}

	public void DoPostConfigure(GameObject go)
	{
		FunkyVestConfig.SetupVest(go);
	}

	public const string ID = "Funky_Vest";
}
