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
		dictionary.Add(SimHashes.Dirt.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("AtmoSuit", global::TUNING.EQUIPMENT.SUITS.SLOT, global::TUNING.EQUIPMENT.SUITS.FABRICATOR, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME, SimHashes.Dirt, dictionary, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_oxygen_kanim", global::TUNING.EQUIPMENT.SUITS.SNAPON, "body_oxygen_kanim", PathFinderFlags.SuitRequired, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
	}

	public const string ID = "AtmoSuit";
}
