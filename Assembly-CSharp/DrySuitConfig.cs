using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class DrySuitConfig : IEquipmentConfig, IHasDlcRestrictions
{
	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.DRY_SUIT;
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("DrySuit", global::TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, (float)global::TUNING.EQUIPMENT.SUITS.DRY_SUIT_MASS, "wetsuit_item_kanim", global::TUNING.EQUIPMENT.VESTS.SNAPON0, "body_wetsuit_kanim", 4, list, global::TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f, new Tag[]
		{
			GameTags.Clothes,
			GameTags.PedestalDisplayable
		}, null);
		int decorMod = ClothingWearer.ClothingInfo.DRY_SUIT.decorMod;
		Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.DRY_SUIT.conductivityMod)), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.DRY_SUIT.conductivityMod)), Descriptor.DescriptorType.Effect, false);
		Descriptor descriptor2 = new Descriptor(string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, decorMod), string.Format("{0}: {1}", DUPLICANTS.ATTRIBUTES.DECOR.NAME, decorMod), Descriptor.DescriptorType.Effect, false);
		equipmentDef.additionalDescriptors.Add(descriptor);
		if (decorMod != 0)
		{
			equipmentDef.additionalDescriptors.Add(descriptor2);
		}
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			ClothingWearer.ClothingInfo.OnEquipVest(eq, clothingInfo);
		};
		equipmentDef.OnUnequipCallBack = new Action<Equippable>(ClothingWearer.ClothingInfo.OnUnequipVest);
		equipmentDef.RecipeDescription = (DlcManager.IsContentSubscribed("DLC3_ID") ? global::STRINGS.EQUIPMENT.PREFABS.DRYSUIT.RECIPE_DESC_DLC3 : global::STRINGS.EQUIPMENT.PREFABS.DRYSUIT.RECIPE_DESC);
		ResourceSet<Effect> effects = Db.Get().effects;
		equipmentDef.EffectImmunites.Add(effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(effects.Get("SoakingWet"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject)
				{
					targetGameObject.AddTag(GameTags.FeetAndWaistProtection);
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner2 = eq.assignee.GetSoleOwner();
				if (soleOwner2 != null)
				{
					GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					if (targetGameObject2)
					{
						targetGameObject2.RemoveTag(GameTags.FeetAndWaistProtection);
					}
				}
			}
		};
		return equipmentDef;
	}

	public static void SetupVest(GameObject go)
	{
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
		DrySuitConfig.SetupVest(go);
	}

	public const string ID = "DrySuit";

	public static ComplexRecipe recipe;
}
