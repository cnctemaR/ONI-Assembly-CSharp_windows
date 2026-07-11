using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class JetSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.Steel.ToString(), 200f);
		dictionary.Add(SimHashes.Petroleum.ToString(), 25f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		string text = "Jet_Suit";
		string slot = global::TUNING.EQUIPMENT.SUITS.SLOT;
		SimHashes simHashes = SimHashes.Steel;
		float num = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS;
		List<AttributeModifier> list2 = list;
		Tag[] array = new Tag[] { GameTags.Suit };
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(text, slot, simHashes, num, "suit_jetpack_kanim", string.Empty, "body_jetpack_kanim", 6, list2, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, array, "JetSuit");
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("Unclean"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component = targetGameObject.GetComponent<Navigator>();
				if (component != null)
				{
					component.SetFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
				MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
				if (component2 != null && component2.HasPerk(RoleManager.rolePerks.ExosuitExpertise.id))
				{
					targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(SuitExpert.AthleticsModifier);
				}
				KAnimControllerBase component3 = targetGameObject.GetComponent<KAnimControllerBase>();
				if (component3)
				{
					component3.AddAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"), 0f);
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner2 = eq.assignee.GetSoleOwner();
				GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Attributes attributes = targetGameObject2.GetAttributes();
				if (attributes != null)
				{
					attributes.Get(Db.Get().Attributes.Athletics).Remove(SuitExpert.AthleticsModifier);
					KAnimControllerBase component4 = targetGameObject2.GetComponent<KAnimControllerBase>();
					if (component4)
					{
						component4.RemoveAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"));
					}
				}
				Navigator component5 = targetGameObject2.GetComponent<Navigator>();
				if (component5 != null)
				{
					component5.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Jet_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		go.AddComponent<JetSuitTank>();
		HelmetController helmetController = go.AddComponent<HelmetController>();
		helmetController.anim_file = "helm_jetpack_kanim";
		helmetController.has_jets = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes);
		component.AddTag(GameTags.PedestalDisplayable);
		go.AddComponent<SuitDiseaseHandler>();
	}

	public const string ID = "Jet_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasJetPack;
}
