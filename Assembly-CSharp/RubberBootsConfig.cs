using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RubberBootsConfig : IEquipmentConfig, IHasDlcRestrictions
{
	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> list = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(RubberBootsConfig.ID, global::TUNING.EQUIPMENT.SHOES.SLOT, SimHashes.Rubber, 30f, "rubber_boots_item_kanim", global::TUNING.EQUIPMENT.SHOES.SNAPON0, "", 6, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.28f, 0.28f, new Tag[]
		{
			GameTags.PedestalDisplayable,
			GameTags.Clothes
		}, null);
		equipmentDef.RecipeDescription = (DlcManager.IsContentSubscribed("DLC3_ID") ? global::STRINGS.EQUIPMENT.PREFABS.RUBBERBOOTS.RECIPE_DESC_DLC3 : global::STRINGS.EQUIPMENT.PREFABS.RUBBERBOOTS.RECIPE_DESC);
		ResourceSet<Effect> effects = Db.Get().effects;
		equipmentDef.EffectImmunites.Add(effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(effects.Get("RecentlySlippedTracker"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject)
				{
					targetGameObject.AddTag(GameTags.FeetProtection);
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
						targetGameObject2.RemoveTag(GameTags.FeetProtection);
					}
				}
			}
		};
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		go.AddOrGet<Equippable>().SetQuality(global::QualityLevel.Poor);
		KBatchedAnimController kbatchedAnimController;
		if (go.TryGetComponent<KBatchedAnimController>(out kbatchedAnimController))
		{
			kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		}
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public static readonly string ID = "RubberBoots";
}
