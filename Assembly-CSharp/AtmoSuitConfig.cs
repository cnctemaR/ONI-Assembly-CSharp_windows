using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AtmoSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.Cuprite.ToString(), 300f);
		dictionary.Add("BasicFabric", 2f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false, true));
		string text = "Atmo_Suit";
		string slot = global::TUNING.EQUIPMENT.SUITS.SLOT;
		string fabricator = global::TUNING.EQUIPMENT.SUITS.FABRICATOR;
		float num = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
		SimHashes simHashes = SimHashes.Dirt;
		Dictionary<string, float> dictionary2 = dictionary;
		float num2 = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS;
		string text2 = "suit_oxygen_kanim";
		string empty = string.Empty;
		string text3 = "body_oxygen_kanim";
		int num3 = 5;
		List<AttributeModifier> list2 = list;
		Tag[] array = new Tag[] { GameTags.Suit };
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(text, slot, fabricator, num, simHashes, dictionary2, num2, text2, empty, text3, num3, list2, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, array);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("Unclean"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			eq.assignee.GetSoleOwner().GetComponent<Navigator>().SetFlags(PathFinder.PotentialPath.Flags.HasSuit | PathFinder.PotentialPath.Flags.UnlimitedSubmergedTravel);
			if (eq.assignee.GetSoleOwner().GetComponent<MinionResume>().HasPerk(RoleManager.rolePerks.ExosuitExpertise.id))
			{
				eq.assignee.GetSoleOwner().GetAttributes().Get(Db.Get().Attributes.Athletics)
					.Add(SuitExpert.AthleticsModifier);
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner = eq.assignee.GetSoleOwner();
				soleOwner.GetAttributes().Get(Db.Get().Attributes.Athletics).Remove(SuitExpert.AthleticsModifier);
				soleOwner.GetComponent<Navigator>().ClearFlags(PathFinder.PotentialPath.Flags.HasSuit | PathFinder.PotentialPath.Flags.UnlimitedSubmergedTravel);
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Atmo_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		go.AddComponent<HelmetController>();
		go.AddComponent<SuitDiseaseHandler>();
	}

	public const string ID = "Atmo_Suit";
}
