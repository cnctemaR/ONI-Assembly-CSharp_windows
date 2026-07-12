using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeadSuitConfig : IEquipmentConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_SCALDING, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScoldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_SCOLDING, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, global::TUNING.EQUIPMENT.SUITS.LEADSUIT_RADIATION_SHIELDING, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.Strength.Id, (float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_STRENGTH, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, global::TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME, false, false, true));
		this.expertAthleticsModifier = new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)(-(float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS), Db.Get().Skills.Suits1.Name, false, false, true);
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Lead_Suit", global::TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_leadsuit_kanim", "", "body_leadsuit_kanim", 6, list, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[]
		{
			GameTags.Suit,
			GameTags.Clothes
		}, null);
		equipmentDef.wornID = "Worn_Lead_Suit";
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner = eq.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component = targetGameObject.GetComponent<Navigator>();
				if (component != null)
				{
					component.SetFlags(PathFinder.PotentialPath.Flags.HasLeadSuit);
				}
				MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
				if (component2 != null && component2.HasPerk(Db.Get().SkillPerks.ExosuitExpertise.Id))
				{
					targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(this.expertAthleticsModifier);
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
						Attributes attributes = targetGameObject2.GetAttributes();
						if (attributes != null)
						{
							attributes.Get(Db.Get().Attributes.Athletics).Remove(this.expertAthleticsModifier);
						}
						Navigator component3 = targetGameObject2.GetComponent<Navigator>();
						if (component3 != null)
						{
							component3.ClearFlags(PathFinder.PotentialPath.Flags.HasLeadSuit);
						}
						Effects component4 = targetGameObject2.GetComponent<Effects>();
						if (component4 != null && component4.HasEffect("SoiledSuit"))
						{
							component4.Remove("SoiledSuit");
						}
					}
					Tag elementTag = eq.GetComponent<SuitTank>().elementTag;
					eq.GetComponent<Storage>().DropUnlessHasTag(elementTag);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Lead_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 40f;
		suitTank.elementTag = GameTags.Breathable;
		go.AddComponent<LeadSuitTank>().batteryDuration = 200f;
		go.AddComponent<HelmetController>();
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes, false);
		component.AddTag(GameTags.PedestalDisplayable, false);
		component.AddTag(GameTags.AirtightSuit, false);
		Durability durability = go.AddComponent<Durability>();
		durability.wornEquipmentPrefabID = "Worn_Lead_Suit";
		durability.durabilityLossPerCycle = global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		go.AddOrGet<AtmoSuit>();
		go.AddComponent<SuitDiseaseHandler>();
	}

	public const string ID = "Lead_Suit";

	public const string WORN_ID = "Worn_Lead_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasLeadSuit;

	private AttributeModifier expertAthleticsModifier;
}
