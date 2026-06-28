using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AquaSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.DirtyWater.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.MAX_UNDERWATER_TRAVELCOST, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME, false, false));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("AquaSuit", global::TUNING.EQUIPMENT.SUITS.SLOT, global::TUNING.EQUIPMENT.SUITS.FABRICATOR, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_FABTIME, SimHashes.Water, dictionary, (float)global::TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS, "suit_water_slow_kanim", global::TUNING.EQUIPMENT.SUITS.SNAPON, "body_water_slow_kanim", PathFinderFlags.None, list, null, false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
	}

	public const string ID = "AquaSuit";
}
